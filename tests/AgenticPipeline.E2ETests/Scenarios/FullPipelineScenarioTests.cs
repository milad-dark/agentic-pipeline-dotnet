namespace AgenticPipeline.E2ETests.Scenarios;

public sealed class FullPipelineScenarioTests
{
    [Fact(Skip = "Requires running dependencies and configured credentials")]
    public Task FullPipeline_HappyPath()
    {
        return Task.CompletedTask;
    }
}
