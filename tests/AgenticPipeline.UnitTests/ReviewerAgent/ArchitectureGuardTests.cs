using AgenticPipeline.Domain.Models;
using AgenticPipeline.ReviewerAgent.Services;
using FluentAssertions;

namespace AgenticPipeline.UnitTests.ReviewerAgent;

public sealed class ArchitectureGuardTests
{
    private readonly ArchitectureGuard _guard = new();

    [Fact]
    public void Check_ShouldFlagInfrastructureReferenceInApplicationLayer()
    {
        var patches = new[]
        {
            new CodePatch { TargetFile = "Application/Service.cs", UnifiedDiff = "+ using Infrastructure.Persistence;" }
        };

        var result = _guard.Check(patches);

        result.HasViolations.Should().BeTrue();
        result.Items.Should().Contain(x => x.Contains("Infrastructure"));
    }

    [Fact]
    public void Check_ShouldFlagDbContextOutsideRepository()
    {
        var patches = new[]
        {
            new CodePatch { TargetFile = "Application/Service.cs", UnifiedDiff = "+ var db = new DbContext();" }
        };

        var result = _guard.Check(patches);

        result.Items.Should().Contain(x => x.Contains("DbContext"));
    }

    [Fact]
    public void Check_ShouldFlagBlockingAsyncCalls()
    {
        var patches = new[]
        {
            new CodePatch { TargetFile = "Application/Service.cs", UnifiedDiff = "+ var x = task.Result;" }
        };

        var result = _guard.Check(patches);

        result.Items.Should().Contain(x => x.Contains("Blocking async"));
    }
}
