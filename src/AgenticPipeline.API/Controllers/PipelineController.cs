using AgenticPipeline.Domain.Models;
using AgenticPipeline.Orchestrator.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgenticPipeline.API.Controllers;

[ApiController]
[Route("api/pipeline")]
public sealed class PipelineController(IPipelineOrchestratorService orchestratorService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<object>> Start([FromBody] JiraTaskInput input, CancellationToken ct)
    {
        var workflowId = await orchestratorService.StartAsync(input, ct);
        return Ok(new { workflowId });
    }

    [HttpGet("{workflowId}/status")]
    public async Task<ActionResult<object>> Status(string workflowId, CancellationToken ct)
    {
        var status = await orchestratorService.GetStatusAsync(workflowId, ct);
        return Ok(new { workflowId, status = status.ToString() });
    }

    [HttpPost("{workflowId}/approve")]
    public async Task<IActionResult> Approve(string workflowId, CancellationToken ct)
    {
        await orchestratorService.ApproveAsync(workflowId, ct);
        return Accepted();
    }

    [HttpPost("{workflowId}/reject")]
    public async Task<IActionResult> Reject(string workflowId, CancellationToken ct)
    {
        await orchestratorService.RejectAsync(workflowId, ct);
        return Accepted();
    }
}
