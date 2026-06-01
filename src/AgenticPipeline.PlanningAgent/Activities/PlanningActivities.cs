using System.Text.Json;
using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Interfaces;
using AgenticPipeline.Domain.Models;
using Microsoft.SemanticKernel;

namespace AgenticPipeline.PlanningAgent.Activities;

[Activity]
public sealed class PlanningActivities(Kernel kernel) : IPlanningAgent
{
    public async Task<ExecutionPlan> DecomposeAsync(SanitizedJiraInput input, CancellationToken ct = default)
        => await DecomposeJiraTaskAsync(input, ct);

    public async Task<ExecutionPlan> DecomposeJiraTaskAsync(SanitizedJiraInput input, CancellationToken ct = default)
    {
        var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "PlannerPrompt.txt");
        var prompt = File.Exists(promptPath)
            ? await File.ReadAllTextAsync(promptPath, ct)
            : "Return execution plan JSON.";

        string? json;
        try
        {
            var function = kernel.CreateFunctionFromPrompt(prompt);
            var result = await kernel.InvokeAsync(function, new KernelArguments
            {
                ["title"] = input.Title,
                ["description"] = input.Description,
                ["criteria"] = input.AcceptanceCriteria
            }, cancellationToken: ct);
            json = result.ToString();
        }
        catch
        {
            json = null;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            return new ExecutionPlan { JiraTicketId = input.TicketId, Title = input.Title };
        }

        var plan = JsonSerializer.Deserialize<ExecutionPlan>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return plan ?? new ExecutionPlan { JiraTicketId = input.TicketId, Title = input.Title };
    }
}
