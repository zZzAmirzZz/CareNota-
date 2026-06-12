using System.Threading.Channels;

namespace CareNota.BackgroundJobs;

public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    // Bounded channel: if the queue fills up (e.g. 500 pending jobs),
    // EnqueueAIJob will block briefly rather than growing unbounded.
    private readonly Channel<(string SasUrl, int VisitId)> _Channel;

    public BackgroundTaskQueue(int Capacity = 500)
    {
        var Options = new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true   // AIBackgroundWorker is the only reader
        };
        _Channel = Channel.CreateBounded<(string, int)>(Options);
    }

    public void EnqueueAIJob(string SasUrl, int VisitId)
    {
        // TryWrite is safe here because FullMode = Wait means the channel
        // will not drop items; it only fails if the channel is completed (shutdown).
        if (!_Channel.Writer.TryWrite((SasUrl, VisitId)))
            throw new InvalidOperationException(
                $"Failed to enqueue AI job for VisitId {VisitId}. Queue may be shutting down.");
    }

    public async Task<(string SasUrl, int VisitId)> DequeueAsync(CancellationToken CancellationToken)
        => await _Channel.Reader.ReadAsync(CancellationToken);
}