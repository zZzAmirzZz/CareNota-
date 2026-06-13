namespace CareNota.BackgroundJobs;

public interface IBackgroundTaskQueue
{
    void EnqueueAIJob(string SasUrl, int VisitId);
    Task<(string SasUrl, int VisitId)> DequeueAsync(CancellationToken CancellationToken);
}