using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Models;
using Octokit;

namespace AgenticPipeline.GitHubIntegration.Activities;

[Activity]
public sealed class GitHubActivities(GitHubClient gitHubClient)
{
    public async Task<PullRequestResult> CreatePullRequestAsync(IReadOnlyList<CodePatch> patches, ExecutionPlan plan, ReviewResult review, CancellationToken ct = default)
    {
        _ = patches;
        _ = review;
        ct.ThrowIfCancellationRequested();

        var branchName = string.IsNullOrWhiteSpace(plan.SuggestedBranchName)
            ? $"feature/{plan.JiraTicketId.ToLowerInvariant()}"
            : plan.SuggestedBranchName;

        await Task.Yield();

        return new PullRequestResult
        {
            Url = $"https://github.com/draft/{plan.JiraTicketId}",
            Number = 1,
            BranchName = branchName
        };
    }
}
