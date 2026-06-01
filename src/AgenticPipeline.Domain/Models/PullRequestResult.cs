namespace AgenticPipeline.Domain.Models;

public sealed class PullRequestResult
{
    public string Url { get; set; } = string.Empty;
    public int Number { get; set; }
    public string BranchName { get; set; } = string.Empty;
}
