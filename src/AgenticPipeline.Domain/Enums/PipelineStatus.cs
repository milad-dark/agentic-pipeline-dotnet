namespace AgenticPipeline.Domain.Enums;

public enum PipelineStatus
{
    Pending,
    Planning,
    Coding,
    Reviewing,
    Testing,
    AwaitingApproval,
    Merged,
    Failed
}
