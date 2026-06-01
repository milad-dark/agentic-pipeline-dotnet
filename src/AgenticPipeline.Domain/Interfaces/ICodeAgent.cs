using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.Domain.Interfaces;

public interface ICodeAgent
{
    Task<IReadOnlyList<CodePatch>> GeneratePatchesAsync(ExecutionPlan plan, RepositoryContext context, CancellationToken ct = default);
}
