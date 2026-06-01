namespace AgenticPipeline.Domain.Models;

public sealed class ArchitectureViolations
{
    public List<string> Items { get; set; } = [];
    public bool HasViolations => Items.Count > 0;
}
