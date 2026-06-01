using Microsoft.Extensions.Logging;

namespace AgenticPipeline.Orchestrator.Activities;

public abstract class BaseActivity<T>(ILogger<T> logger)
{
    protected ILogger<T> Logger { get; } = logger;
}
