namespace AgenticPipeline.Domain.Models;

public sealed class MergeRequestResult
{
    public string PrUrl { get; set; } = string.Empty;
    public bool Approved { get; set; }
}
