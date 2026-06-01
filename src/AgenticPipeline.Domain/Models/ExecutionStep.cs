namespace AgenticPipeline.Domain.Models;

public sealed class ExecutionStep
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetFile { get; set; } = string.Empty;
    public int Order { get; set; }
}
