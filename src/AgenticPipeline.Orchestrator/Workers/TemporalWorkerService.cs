using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AgenticPipeline.Orchestrator.Workers;

public sealed class TemporalWorkerService(ILogger<TemporalWorkerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Temporal worker started on task queue: agentic-pipeline");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
