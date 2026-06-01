namespace AgenticPipeline.Domain.Models;

public sealed class CodePatch
{
    public string StepId { get; set; } = string.Empty;
    public string TargetFile { get; set; } = string.Empty;
    public string UnifiedDiff { get; set; } = string.Empty;
    public DateTimeOffset? AppliedAt { get; set; }
}
