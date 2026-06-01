namespace AgenticPipeline.Domain.Models;

public sealed class SanitizedJiraInput
{
    public string TicketId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
}
