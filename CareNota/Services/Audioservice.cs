//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using CareNota.DTOs.Audio;
//using CareNota.Models;
//using CareNota.Repositories.Interfaces;
//using CareNota.Services.Interfaces;

//namespace CareNota.Services;

//public class AudioService : IAudioService
//{
//    private readonly IAudioRepository _AudioRepo;
//    private readonly IVisitRepository _VisitRepo;
//    //private readonly IAIService _AiService;
//    private readonly BlobServiceClient _BlobClient;
//    private readonly IConfiguration _Config;
//    private readonly ILogger<AudioService> _Logger;

//    // How long to keep the audio file after AI finishes (from appsettings)
//    // "AudioSettings:RetentionHours" → default 1 hour
//    private int RetentionHours =>
//        int.Parse(_Config["AudioSettings:RetentionHours"] ?? "1");

//    private string ContainerName =>
//        _Config["AudioSettings:ContainerName"] ?? "carenota-audio";

//    public AudioService(
//        IAudioRepository AudioRepo,
//        IVisitRepository VisitRepo,
//        //IAIService AiService,
//        BlobServiceClient BlobClient,
//        IConfiguration Config,
//        ILogger<AudioService> Logger)
//    {
//        _AudioRepo = AudioRepo;
//        _VisitRepo = VisitRepo;
//        //_AiService = AiService;
//        _BlobClient = BlobClient;
//        _Config = Config;
//        _Logger = Logger;
//    }

//    // ── Upload ────────────────────────────────────────────────────────────────
//    public async Task<AudioRecordDto> UploadAsync(int VisitId, IFormFile AudioFile)
//    {
//        // 1. Validate visit exists
//        if (!await _VisitRepo.ExistsAsync(V => V.VisitID == VisitId))
//            throw new KeyNotFoundException($"Visit {VisitId} not found.");

//        // 2. Prevent duplicate uploads for the same visit
//        var Existing = await _AudioRepo.GetByVisitIdAsync(VisitId);
//        if (Existing is not null)
//            throw new InvalidOperationException(
//                "An audio file already exists for this visit. Delete it first.");

//        // 3. Upload file to Azure Blob Storage
//        var BlobUrl = await UploadToBlobAsync(AudioFile, VisitId);

//        // 4. Save AudioRecord with scheduled deletion time
//        var AudioRecord = new AudioRecord
//        {
//            AudioFileURL = BlobUrl,
//            CreatedAt = DateTime.UtcNow,
//            DeletionAt = DateTime.UtcNow.AddHours(RetentionHours),
//            VisitID = VisitId
//        };

//        await _AudioRepo.AddAsync(AudioRecord);
//        await _AudioRepo.SaveChangesAsync();

//        _Logger.LogInformation(
//            "Audio uploaded for Visit {VisitId}. Blob: {Url}. Scheduled deletion: {Time}",
//            VisitId, BlobUrl, AudioRecord.DeletionAt);

//        // 5. Trigger AI processing in the background (fire-and-forget)
//        _ = Task.Run(async () =>
//        {
//            //try
//            //{
//            //    await _AiService.ProcessAudioAsync(BlobUrl, VisitId);
//            //}
//            //catch (Exception Ex)
//            //{
//            //    _Logger.LogError(Ex,
//            //        "AI processing failed for Visit {VisitId}", VisitId);
//            //}
//        });

//        return MapToDto(AudioRecord, "Processing");
//    }

//    // ── Get Status ────────────────────────────────────────────────────────────
//    public async Task<AudioRecordDto?> GetByVisitIdAsync(int VisitId)
//    {
//        var Record = await _AudioRepo.GetByVisitIdAsync(VisitId);
//        if (Record is null) return null;

//        // Derive status from DeletionAt — if it's past, AI is done and cleanup pending
//        var Status = Record.DeletionAt > DateTime.UtcNow ? "Processing" : "Done";
//        return MapToDto(Record, Status);
//    }

//    // ── Background Cleanup ────────────────────────────────────────────────────
//    public async Task DeleteExpiredAudioAsync()
//    {
//        var ExpiredRecords = await _AudioRepo.GetExpiredRecordsAsync();

//        foreach (var Record in ExpiredRecords)
//        {
//            try
//            {
//                await DeleteBlobAsync(Record.AudioFileURL);
//                _AudioRepo.Remove(Record);

//                _Logger.LogInformation(
//                    "Deleted expired audio for Visit {VisitId}. Blob: {Url}",
//                    Record.VisitID, Record.AudioFileURL);
//            }
//            catch (Exception Ex)
//            {
//                _Logger.LogError(Ex,
//                    "Failed to delete blob for Visit {VisitId}", Record.VisitID);
//            }
//        }

//        await _AudioRepo.SaveChangesAsync();
//    }

//    // ── Azure Blob Helpers ────────────────────────────────────────────────────
//    private async Task<string> UploadToBlobAsync(IFormFile File, int VisitId)
//    {
//        var Container = _BlobClient.GetBlobContainerClient(ContainerName);
//        await Container.CreateIfNotExistsAsync(PublicAccessType.None);

//        // Unique blob name: audio/visitId_timestamp.ext
//        var Extension = Path.GetExtension(File.FileName).ToLowerInvariant();
//        var BlobName = $"audio/{VisitId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Extension}";
//        var BlobRef = Container.GetBlobClient(BlobName);

//        var BlobHttpHeaders = new BlobHttpHeaders
//        {
//            ContentType = File.ContentType
//        };

//        await using var Stream = File.OpenReadStream();
//        await BlobRef.UploadAsync(Stream, new BlobUploadOptions
//        {
//            HttpHeaders = BlobHttpHeaders
//        });

//        return BlobRef.Uri.ToString();
//    }

//    private async Task DeleteBlobAsync(string BlobUrl)
//    {
//        if (string.IsNullOrEmpty(BlobUrl)) return;

//        // Extract blob name from full URL
//        var Uri = new Uri(BlobUrl);
//        var BlobName = string.Join("", Uri.Segments[2..]);

//        var Container = _BlobClient.GetBlobContainerClient(ContainerName);
//        await Container.DeleteBlobIfExistsAsync(BlobName);
//    }

//    private static AudioRecordDto MapToDto(AudioRecord Record, string Status)
//        => new()
//        {
//            AudioID = Record.AudioID,
//            AudioFileURL = Record.AudioFileURL,
//            CreatedAt = Record.CreatedAt,
//            DeletionAt = Record.DeletionAt,
//            VisitID = Record.VisitID,
//            Status = Status
//        };
//}