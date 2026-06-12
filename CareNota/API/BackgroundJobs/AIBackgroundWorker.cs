using CareNota.Interfaces;
using CareNota.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareNota.BackgroundJobs;

public sealed class AIBackgroundWorker : BackgroundService
{
    private readonly IBackgroundTaskQueue _Queue;
    private readonly IServiceScopeFactory _ScopeFactory;
    private readonly ILogger<AIBackgroundWorker> _Logger;

    public AIBackgroundWorker(
        IBackgroundTaskQueue Queue,
        IServiceScopeFactory ScopeFactory,
        ILogger<AIBackgroundWorker> Logger)
    {
        _Queue = Queue;
        _ScopeFactory = ScopeFactory;
        _Logger = Logger;
    }

    protected override async Task ExecuteAsync(CancellationToken StoppingToken)
    {
        _Logger.LogInformation("AIBackgroundWorker started.");

        // Keep looping until the host requests shutdown
        while (!StoppingToken.IsCancellationRequested)
        {
            (string SasUrl, int VisitId) job = default;

            try
            {
                // ── Wait for the next job ─────────────────────────────────────
                job = await _Queue.DequeueAsync(StoppingToken);

                _Logger.LogInformation(
                    "[AI Worker] Job started — VisitId={VisitId}", job.VisitId);

                // ── Create a fresh DI scope for every job ─────────────────────
                // This gives us a brand-new, non-disposed ApplicationDbContext
                // along with all scoped repositories and services.
                await using var Scope = _ScopeFactory.CreateAsyncScope();
                var AiService = Scope.ServiceProvider.GetRequiredService<IAIService>();

                await AiService.ProcessAudioAsync(job.SasUrl, job.VisitId);

                _Logger.LogInformation(
                    "[AI Worker] Job completed — VisitId={VisitId}", job.VisitId);
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown — stop the loop cleanly
                _Logger.LogInformation("AIBackgroundWorker is stopping.");
                break;
            }
            catch (Exception Ex)
            {
                // Log the failure but keep the worker alive for the next job
                _Logger.LogError(Ex,
                    "[AI Worker] Job failed — VisitId={VisitId}", job.VisitId);
            }
        }

        _Logger.LogInformation("AIBackgroundWorker stopped.");
    }
}