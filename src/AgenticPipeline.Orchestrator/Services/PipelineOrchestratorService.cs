using AgenticPipeline.Domain.Enums;
using AgenticPipeline.Domain.Models;
using AgenticPipeline.Orchestrator.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace AgenticPipeline.Orchestrator.Services;

public interface IPipelineOrchestratorService
{
    Task<string> StartAsync(JiraTaskInput input, CancellationToken ct = default);
    Task<PipelineStatus> GetStatusAsync(string workflowId, CancellationToken ct = default);
    Task ApproveAsync(string workflowId, CancellationToken ct = default);
    Task RejectAsync(string workflowId, CancellationToken ct = default);
}

public sealed class PipelineOrchestratorService(IServiceProvider serviceProvider) : IPipelineOrchestratorService
{
    private readonly Dictionary<string, (JiraToMergeRequestWorkflow Workflow, PipelineStatus Status)> _workflows = [];

    public Task<string> StartAsync(JiraTaskInput input, CancellationToken ct = default)
    {
        var workflowId = $"pipeline-{Guid.NewGuid():N}";
        var workflow = serviceProvider.GetRequiredService<JiraToMergeRequestWorkflow>();

        _workflows[workflowId] = (workflow, PipelineStatus.Pending);

        _ = Task.Run(async () =>
        {
            try
            {
                await workflow.RunAsync(input, ct);
                _workflows[workflowId] = (workflow, workflow.Status);
            }
            catch
            {
                _workflows[workflowId] = (workflow, PipelineStatus.Failed);
            }
        }, ct);

        return Task.FromResult(workflowId);
    }

    public Task<PipelineStatus> GetStatusAsync(string workflowId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(_workflows.TryGetValue(workflowId, out var workflow) ? workflow.Workflow.Status : PipelineStatus.Failed);
    }

    public async Task ApproveAsync(string workflowId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_workflows.TryGetValue(workflowId, out var workflow))
        {
            await workflow.Workflow.ApproveAsync();
        }
    }

    public async Task RejectAsync(string workflowId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_workflows.TryGetValue(workflowId, out var workflow))
        {
            await workflow.Workflow.RejectAsync();
        }
    }
}
