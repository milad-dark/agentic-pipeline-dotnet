using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.Domain.Interfaces;

public interface IPlanningAgent
{
    Task<ExecutionPlan> DecomposeAsync(SanitizedJiraInput input, CancellationToken ct = default);
}
