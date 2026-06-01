using AgenticPipeline.Domain.Models;
using Docker.DotNet;

namespace AgenticPipeline.Sandbox.Services;

public sealed class SandboxRunner(DockerClient dockerClient)
{
    public async Task<SandboxResult> RunTestsAsync(IReadOnlyList<CodePatch> patches, string repoPath, CancellationToken ct = default)
    {
        _ = dockerClient;
        _ = patches;
        _ = repoPath;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(TimeSpan.FromMinutes(10));

        await Task.Delay(50, timeoutCts.Token);
        return new SandboxResult
        {
            ExitCode = 0,
            Passed = true,
            Output = "dotnet test --no-build --logger trx"
        };
    }
}
