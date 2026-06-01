namespace AgenticPipeline.Domain.Models;

public sealed class SandboxResult
{
    public int ExitCode { get; set; }
    public bool Passed { get; set; }
    public string? Output { get; set; }
}
