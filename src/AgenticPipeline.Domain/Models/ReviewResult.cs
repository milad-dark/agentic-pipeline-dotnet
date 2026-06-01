namespace AgenticPipeline.Domain.Models;

public sealed class ReviewResult
{
    public List<string> Issues { get; set; } = [];
    public string Severity { get; set; } = "low";
    public bool Approved { get; set; } = true;
    public string Summary { get; set; } = string.Empty;
}
