namespace AgenticPipeline.IntegrationTests.Workflows;

public sealed class JiraToMergeRequestWorkflowTests
{
    [Fact(Skip = "Requires full Temporal and infra stack")]
    public Task Workflow_HappyPath_Completes()
    {
        return Task.CompletedTask;
    }
}
