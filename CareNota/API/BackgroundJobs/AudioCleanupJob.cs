using Azure.Storage.Blobs;
using CareNota.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareNota.API.BackgroundJobs;

public class AudioCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AudioCleanupJob> _logger;

    public AudioCleanupJob(
        IServiceScopeFactory scopeFactory,
        BlobServiceClient blobServiceClient,
        IConfiguration configuration,
        ILogger<AudioCleanupJob> logger)
    {
        _scopeFactory = scopeFactory;
        _blobServiceClient = blobServiceClient;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(_configuration.GetValue<int>("AudioSettings:CleanupIntervalMinutes", 15));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AudioCleanupJob");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var audioRepo = scope.ServiceProvider.GetRequiredService<IAudioRepository>();

        var pending = await audioRepo.GetPendingDeletionsAsync();
        if (pending.Count == 0) return;

        var containerName = _configuration["AzureBlob:ContainerName"] ?? "audio-files";
        var container = _blobServiceClient.GetBlobContainerClient(containerName);

        foreach (var record in pending)
        {
            try
            {
                var blobName = ExtractBlobName(record.AudioFileURL, containerName);
                var blobClient = container.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();

                audioRepo.Delete(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete AudioId={AudioId}", record.AudioID);
            }
        }

        await audioRepo.SaveAsync();
    }

    private static string ExtractBlobName(string blobUrl, string containerName)
    {
        var uri = new Uri(blobUrl);
        var prefix = $"/{containerName}/";
        var path = uri.AbsolutePath;
        return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? path[prefix.Length..]
            : path.TrimStart('/');
    }
}