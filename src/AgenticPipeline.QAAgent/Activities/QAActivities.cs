using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Interfaces;
using AgenticPipeline.Domain.Models;
using AgenticPipeline.QAAgent.Services;
using AgenticPipeline.Sandbox.Services;

namespace AgenticPipeline.QAAgent.Activities;

[Activity]
public sealed class QAActivities(SandboxRunner sandboxRunner, TestResultParser parser) : IQAAgent
{
    public async Task<QAResult> RunInSandboxAsync(IReadOnlyList<CodePatch> patches, CancellationToken ct = default)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var sandbox = await sandboxRunner.RunTestsAsync(patches, Directory.GetCurrentDirectory(), ct);

        var result = new QAResult
        {
            Passed = sandbox.Passed,
            FailureReason = sandbox.Passed ? null : sandbox.Output,
            TestsRun = 0,
            TestsPassed = 0,
            TestsFailed = sandbox.Passed ? 0 : 1,
            Duration = DateTimeOffset.UtcNow - startedAt
        };

        var trxPath = "/tmp/results/test_results.trx";
        if (File.Exists(trxPath))
        {
            var parsed = parser.ParseTrx(trxPath);
            parsed.Duration = result.Duration;
            return parsed;
        }

        return result;
    }
}
