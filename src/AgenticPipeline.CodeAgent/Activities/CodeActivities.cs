using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Interfaces;
using AgenticPipeline.Domain.Models;
using Microsoft.SemanticKernel;

namespace AgenticPipeline.CodeAgent.Activities;

[Activity]
public sealed class CodeActivities(Kernel kernel) : ICodeAgent
{
    public async Task<IReadOnlyList<CodePatch>> GeneratePatchesAsync(ExecutionPlan plan, RepositoryContext context, CancellationToken ct = default)
    {
        var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "PatchGenerationPrompt.txt");
        var prompt = File.Exists(promptPath)
            ? await File.ReadAllTextAsync(promptPath, ct)
            : "Return a unified diff patch";

        var function = kernel.CreateFunctionFromPrompt(prompt);
        var interfaces = string.Join("\n", context.Interfaces);
        var contextBody = string.Join("\n", context.RelevantFiles);

        var patches = new List<CodePatch>();
        foreach (var step in plan.ExecutionSteps.OrderBy(s => s.Order))
        {
            var result = await kernel.InvokeAsync(function, new KernelArguments
            {
                ["task"] = step.Description,
                ["context"] = contextBody,
                ["interfaces"] = interfaces
            }, cancellationToken: ct);

            patches.Add(new CodePatch
            {
                StepId = step.Id,
                TargetFile = step.TargetFile,
                UnifiedDiff = result.ToString(),
                AppliedAt = DateTimeOffset.UtcNow
            });
        }

        return patches;
    }
}
