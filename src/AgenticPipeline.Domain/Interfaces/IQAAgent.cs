using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.Domain.Interfaces;

public interface IQAAgent
{
    Task<QAResult> RunInSandboxAsync(IReadOnlyList<CodePatch> patches, CancellationToken ct = default);
}
