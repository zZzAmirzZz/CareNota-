using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CareNota.BackgroundJobs;          
using CareNota.DTOs.Audio;
using CareNota.Interfaces;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using CareNota.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CareNota.Services;

public class AudioService : IAudioService
{
    private readonly BlobServiceClient _BlobServiceClient;
    private readonly IAudioRepository _AudioRepository;
    private readonly IBackgroundTaskQueue _TaskQueue;       
    private readonly IValidator<AudioUploadDto> _Validator;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AudioService> _Logger;

    public AudioService(
        BlobServiceClient BlobServiceClient,
        IAudioRepository AudioRepository,
        IBackgroundTaskQueue TaskQueue,                     // ← replaces IAIService
        IValidator<AudioUploadDto> Validator,
        IConfiguration Configuration,
        ILogger<AudioService> Logger)
    {
        _BlobServiceClient = BlobServiceClient;
        _AudioRepository = AudioRepository;
        _TaskQueue = TaskQueue;
        _Validator = Validator;
        _Configuration = Configuration;
        _Logger = Logger;
    }

    public async Task<AudioRecordResponseDto> UploadAudioAsync(IFormFile File, int VisitId)
    {
        // ── Validate ──────────────────────────────────────────────────────────
        var Dto = new AudioUploadDto { AudioFile = File, VisitId = VisitId };
        var ValidationResult = await _Validator.ValidateAsync(Dto);
        if (!ValidationResult.IsValid)
            throw new ValidationException(string.Join("; ", ValidationResult.Errors));

        // ── Upload to Azure Blob (private container) ──────────────────────────
        var ContainerName = _Configuration["AzureBlob:ContainerName"] ?? "audio-files";
        var ContainerClient = _BlobServiceClient.GetBlobContainerClient(ContainerName);
        await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var Extension = Path.GetExtension(File.FileName).ToLowerInvariant();
        var BlobName = $"visits/{VisitId}/{Guid.NewGuid()}{Extension}";
        var BlobClient = ContainerClient.GetBlobClient(BlobName);

        await using var Stream = File.OpenReadStream();
        await BlobClient.UploadAsync(Stream, new BlobHttpHeaders { ContentType = File.ContentType });

        var AudioUrl = BlobClient.Uri.ToString();

        // ── Generate SAS URL for FastAPI ──────────────────────────────────────
        var SasExpiryHours = _Configuration.GetValue<int>("AudioSettings:SasExpiryHours", 2);
        var SasBuilder = new BlobSasBuilder
        {
            BlobContainerName = BlobClient.BlobContainerName,
            BlobName = BlobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(SasExpiryHours)
        };
        SasBuilder.SetPermissions(BlobSasPermissions.Read);
        var SasUrl = BlobClient.GenerateSasUri(SasBuilder).ToString();

        // ── Save AudioRecord (plain URL — no SAS token in DB) ─────────────────
        var DeletionHours = _Configuration.GetValue<int>("AudioSettings:DeletionDelayHours", 1);
        var AudioRecord = new AudioRecord
        {
            AudioFileURL = AudioUrl,
            VisitID = VisitId,
            CreatedAt = DateTime.UtcNow,
            DeletionAt = DateTime.UtcNow.AddHours(DeletionHours)
        };

        await _AudioRepository.AddAsync(AudioRecord);
        await _AudioRepository.SaveAsync();

        // ── Enqueue AI job (safe — no Task.Run, no scope leak) ────────────────
        _TaskQueue.EnqueueAIJob(SasUrl, VisitId);
        _Logger.LogInformation(
            "[AudioService] AI job enqueued — VisitId={VisitId}", VisitId);

        return new AudioRecordResponseDto
        {
            AudioId = AudioRecord.AudioID,
            AudioFileUrl = AudioUrl,
            CreatedAt = AudioRecord.CreatedAt,
            DeletionAt = AudioRecord.DeletionAt,
            VisitId = VisitId,
            Message = "Audio uploaded successfully. AI processing queued."
        };
    }
}