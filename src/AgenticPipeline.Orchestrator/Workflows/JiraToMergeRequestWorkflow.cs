using AgenticPipeline.CodeAgent.Activities;
using AgenticPipeline.ContextAgent.Activities;
using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Enums;
using AgenticPipeline.Domain.Models;
using AgenticPipeline.GitHubIntegration.Activities;
using AgenticPipeline.Orchestrator.Activities;
using AgenticPipeline.PlanningAgent.Activities;
using AgenticPipeline.QAAgent.Activities;
using AgenticPipeline.ReviewerAgent.Activities;

namespace AgenticPipeline.Orchestrator.Workflows;

[Workflow]
public sealed class JiraToMergeRequestWorkflow(
    SecurityActivities securityActivities,
    PlanningActivities planningActivities,
    ContextActivities contextActivities,
    CodeActivities codeActivities,
    QAActivities qaActivities,
    ReviewerActivities reviewerActivities,
    GitHubActivities gitHubActivities)
{
    private bool _approved;
    private bool _rejected;

    public PipelineStatus Status { get; private set; } = PipelineStatus.Pending;

    public async Task<MergeRequestResult> RunAsync(JiraTaskInput input, CancellationToken ct = default)
    {
        var sanitized = await securityActivities.SanitizeAndValidateAsync(input, ct);

        Status = PipelineStatus.Planning;
        var plan = await planningActivities.DecomposeJiraTaskAsync(sanitized, ct);

        Status = PipelineStatus.Coding;
        var context = await contextActivities.BuildRepositoryContextAsync(plan, ct);
        var patches = await codeActivities.GeneratePatchesAsync(plan, context, ct);

        Status = PipelineStatus.Testing;
        var qa = await qaActivities.RunInSandboxAsync(patches, ct);
        if (!qa.Passed)
        {
            Status = PipelineStatus.Failed;
            return new MergeRequestResult { Approved = false, PrUrl = string.Empty };
        }

        Status = PipelineStatus.Reviewing;
        var review = await reviewerActivities.ReviewPatchesAsync(patches, context, ct);
        if (!review.Approved)
        {
            Status = PipelineStatus.Failed;
            return new MergeRequestResult { Approved = false, PrUrl = string.Empty };
        }

        var pr = await gitHubActivities.CreatePullRequestAsync(patches, plan, review, ct);

        Status = PipelineStatus.AwaitingApproval;
        await WaitForHumanDecisionAsync(TimeSpan.FromHours(48), ct);

        if (_rejected)
        {
            Status = PipelineStatus.Failed;
            return new MergeRequestResult { Approved = false, PrUrl = pr.Url };
        }

        Status = PipelineStatus.Merged;
        return new MergeRequestResult { Approved = _approved, PrUrl = pr.Url };
    }

    public Task ApproveAsync()
    {
        _approved = true;
        _rejected = false;
        return Task.CompletedTask;
    }

    public Task RejectAsync()
    {
        _approved = false;
        _rejected = true;
        return Task.CompletedTask;
    }

    private async Task WaitForHumanDecisionAsync(TimeSpan timeout, CancellationToken ct)
    {
        var start = DateTimeOffset.UtcNow;
        while (!_approved && !_rejected)
        {
            ct.ThrowIfCancellationRequested();
            if (DateTimeOffset.UtcNow - start > timeout)
            {
                _rejected = true;
                break;
            }

            await Task.Delay(500, ct);
        }
    }
}
