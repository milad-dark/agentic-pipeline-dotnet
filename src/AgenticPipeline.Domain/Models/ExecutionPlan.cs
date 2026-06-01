using AgenticPipeline.Domain.Enums;

namespace AgenticPipeline.Domain.Models;

public sealed class ExecutionPlan
{
    public string JiraTicketId { get; set; } = string.Empty;
    public string JiraUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> AffectedRepositories { get; set; } = [];
    public List<string> ImpactedModules { get; set; } = [];
    public TaskComplexity Complexity { get; set; } = TaskComplexity.Medium;
    public List<string> MissingRequirements { get; set; } = [];
    public string SuggestedBranchName { get; set; } = string.Empty;
    public List<ExecutionStep> ExecutionSteps { get; set; } = [];
    public string ValidationStrategy { get; set; } = string.Empty;
}
