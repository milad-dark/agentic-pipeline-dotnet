using System.Text.Json;
using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Interfaces;
using AgenticPipeline.Domain.Models;
using AgenticPipeline.ReviewerAgent.Services;
using Microsoft.SemanticKernel;

namespace AgenticPipeline.ReviewerAgent.Activities;

[Activity]
public sealed class ReviewerActivities(ArchitectureGuard architectureGuard, Kernel kernel) : IReviewerAgent
{
    public async Task<ReviewResult> ReviewPatchesAsync(IReadOnlyList<CodePatch> patches, RepositoryContext context, CancellationToken ct = default)
    {
        var violations = architectureGuard.Check(patches);
        var baseResult = new ReviewResult
        {
            Issues = violations.Items,
            Severity = violations.HasViolations ? "high" : "low",
            Approved = !violations.HasViolations,
            Summary = violations.HasViolations ? "Architecture violations detected." : "No architecture violations detected."
        };

        var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "ReviewerPrompt.txt");
        var prompt = File.Exists(promptPath)
            ? await File.ReadAllTextAsync(promptPath, ct)
            : "Review diff and return json";

        var function = kernel.CreateFunctionFromPrompt(prompt);
        var result = await kernel.InvokeAsync(function, new KernelArguments
        {
            ["context"] = string.Join("\n", context.RelevantFiles),
            ["diff"] = string.Join("\n", patches.Select(p => p.UnifiedDiff))
        }, cancellationToken: ct);

        var parsed = JsonSerializer.Deserialize<ReviewResult>(result.ToString() ?? string.Empty, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (parsed is null)
        {
            return baseResult;
        }

        parsed.Issues = baseResult.Issues.Concat(parsed.Issues).Distinct().ToList();
        parsed.Approved &= baseResult.Approved;
        if (baseResult.Issues.Count > 0)
        {
            parsed.Severity = "high";
        }

        return parsed;
    }
}
