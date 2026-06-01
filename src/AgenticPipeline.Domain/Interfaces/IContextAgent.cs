using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.Domain.Interfaces;

public interface IContextAgent
{
    Task<RepositoryContext> BuildRepositoryContextAsync(ExecutionPlan plan, CancellationToken ct = default);
}
