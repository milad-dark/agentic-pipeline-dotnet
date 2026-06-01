using Atlassian.Jira;
using Microsoft.Extensions.Configuration;

namespace AgenticPipeline.PlanningAgent.Services;

public sealed class JiraService
{
    private readonly Jira _client;

    public JiraService(IConfiguration configuration)
    {
        var baseUrl = configuration["Jira:BaseUrl"] ?? string.Empty;
        var username = configuration["Jira:Username"] ?? string.Empty;
        var apiToken = configuration["Jira:ApiToken"] ?? string.Empty;
        _client = Jira.CreateRestClient(baseUrl, username, apiToken);
    }

    public async Task<Issue> GetIssueAsync(string ticketId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await _client.Issues.GetIssueAsync(ticketId);
    }
}
