using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Interfaces;
using AgenticPipeline.Domain.Models;
using AgenticPipeline.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgenticPipeline.ContextAgent.Activities;

[Activity]
public sealed class ContextActivities(VectorDbContext dbContext) : IContextAgent
{
    public async Task<RepositoryContext> BuildRepositoryContextAsync(ExecutionPlan plan, CancellationToken ct = default)
    {
        var chunks = await dbContext.CodeChunks
            .AsNoTracking()
            .Take(200)
            .ToListAsync(ct);

        var selected = chunks
            .Where(c => plan.ImpactedModules.Count == 0 || plan.ImpactedModules.Any(m => c.FilePath.Contains(m, StringComparison.OrdinalIgnoreCase) || c.SymbolName.Contains(m, StringComparison.OrdinalIgnoreCase)))
            .Take(20)
            .ToList();

        return new RepositoryContext
        {
            RelevantFiles = selected.Select(c => c.FilePath).Distinct().ToList(),
            Interfaces = selected.Where(c => c.ChunkType.Contains("Interface", StringComparison.OrdinalIgnoreCase)).Select(c => c.SymbolName).Distinct().ToList(),
            RelatedTests = selected.Where(c => c.FilePath.Contains("Test", StringComparison.OrdinalIgnoreCase)).Select(c => c.FilePath).Distinct().ToList()
        };
    }
}
