namespace AgenticPipeline.Infrastructure.Messaging;

public sealed record PipelineStartedEvent(string WorkflowId, string JiraTicketId, DateTimeOffset StartedAt);
public sealed record PipelineCompletedEvent(string WorkflowId, string PullRequestUrl, DateTimeOffset CompletedAt);
public sealed record PipelineFailedEvent(string WorkflowId, string Reason, DateTimeOffset FailedAt);
