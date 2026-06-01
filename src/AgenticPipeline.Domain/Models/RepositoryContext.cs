namespace AgenticPipeline.Domain.Models;

public sealed class RepositoryContext
{
    public List<string> RelevantFiles { get; set; } = [];
    public List<string> Interfaces { get; set; } = [];
    public List<string> RelatedTests { get; set; } = [];
}
