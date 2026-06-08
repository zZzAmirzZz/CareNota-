using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;                    // ← NEW
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
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IAudioRepository _audioRepository;
    private readonly IAIService _aiService;
    private readonly IValidator<AudioUploadDto> _validator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AudioService> _logger;

    public AudioService(
        BlobServiceClient blobServiceClient,
        IAudioRepository audioRepository,
        IAIService aiService,
        IValidator<AudioUploadDto> validator,
        IConfiguration configuration,
        ILogger<AudioService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _audioRepository = audioRepository;
        _aiService = aiService;
        _validator = validator;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AudioRecordResponseDto> UploadAudioAsync(IFormFile file, int visitId)
    {
        // ── Validate ──────────────────────────────────────────────────────────
        var dto = new AudioUploadDto { AudioFile = file, VisitId = visitId };
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(string.Join("; ", validationResult.Errors));

        // ── Upload to Azure Blob (private container) ──────────────────────────
        var containerName = _configuration["AzureBlob:ContainerName"] ?? "audio-files";
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blobName = $"visits/{visitId}/{Guid.NewGuid()}{extension}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        // Plain URL — stored in DB and used by the cleanup job
        var audioUrl = blobClient.Uri.ToString();

        // ── Generate SAS URL for FastAPI ──────────────────────────────────────
        // Container is private so FastAPI cannot access the plain URL.
        // SAS URL is read-only and time-limited — never stored in the DB.
        var sasExpiryHours = _configuration.GetValue<int>("AudioSettings:SasExpiryHours", 2);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(sasExpiryHours)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        var sasUrl = blobClient.GenerateSasUri(sasBuilder).ToString();
        _logger.LogInformation("SAS URL generated: {SasUrl}", sasUrl);

        // ── Save AudioRecord (plain URL — no SAS token in DB) ─────────────────
        var deletionHours = _configuration.GetValue<int>("AudioSettings:DeletionDelayHours", 1);
        var audioRecord = new AudioRecord
        {
            AudioFileURL = audioUrl,   // plain URL
            VisitID = visitId,
            CreatedAt = DateTime.UtcNow,
            DeletionAt = DateTime.UtcNow.AddHours(deletionHours)
        };

        await _audioRepository.AddAsync(audioRecord);
        await _audioRepository.SaveAsync();

        // ── Fire AI with SAS URL (non-blocking) ───────────────────────────────
        _ = Task.Run(async () =>
        {
            try { await _aiService.ProcessAudioAsync(sasUrl, visitId); }  // ← sasUrl not audioUrl
            catch (Exception ex) { _logger.LogError(ex, "AI processing failed for Visit {VisitId}", visitId); }
        });

        return new AudioRecordResponseDto
        {
            AudioId = audioRecord.AudioID,
            AudioFileUrl = audioUrl,   // return plain URL to frontend
            CreatedAt = audioRecord.CreatedAt,
            DeletionAt = audioRecord.DeletionAt,
            VisitId = visitId,
            Message = "Audio uploaded successfully. AI processing started."
        };
    }
}