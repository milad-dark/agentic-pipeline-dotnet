using AgenticPipeline.Domain.Models;
using AgenticPipeline.PlanningAgent.Activities;
using FluentAssertions;
using Microsoft.SemanticKernel;

namespace AgenticPipeline.UnitTests.PlanningAgent;

public sealed class PlanningActivitiesTests
{
    [Fact]
    public async Task DecomposeJiraTaskAsync_ShouldReturnFallbackPlan_WhenModelOutputUnavailable()
    {
        var kernel = Kernel.CreateBuilder().Build();
        var activities = new PlanningActivities(kernel);

        var input = new SanitizedJiraInput
        {
            TicketId = "AP-123",
            Title = "[USER_INPUT_START]title[USER_INPUT_END]",
            Description = "[USER_INPUT_START]desc[USER_INPUT_END]",
            AcceptanceCriteria = "[USER_INPUT_START]ac[USER_INPUT_END]"
        };

        var plan = await activities.DecomposeJiraTaskAsync(input);

        plan.JiraTicketId.Should().Be("AP-123");
    }
}
