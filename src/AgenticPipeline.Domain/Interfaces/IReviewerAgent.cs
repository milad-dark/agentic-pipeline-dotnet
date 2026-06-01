using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.Domain.Interfaces;

public interface IReviewerAgent
{
    Task<ReviewResult> ReviewPatchesAsync(IReadOnlyList<CodePatch> patches, RepositoryContext context, CancellationToken ct = default);
}
