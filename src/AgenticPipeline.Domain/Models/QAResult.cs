namespace AgenticPipeline.Domain.Models;

public sealed class QAResult
{
    public bool Passed { get; set; }
    public string? FailureReason { get; set; }
    public int TestsRun { get; set; }
    public int TestsPassed { get; set; }
    public int TestsFailed { get; set; }
    public TimeSpan Duration { get; set; }
}
