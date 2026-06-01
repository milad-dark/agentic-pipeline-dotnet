using AgenticPipeline.Domain.Models;
using AgenticPipeline.Security.Exceptions;
using Microsoft.Extensions.Logging;

namespace AgenticPipeline.Security.Services;

public sealed class InputSanitizer(ILogger<InputSanitizer> logger)
{
    private static readonly string[] PromptInjectionPatterns =
    [
        "ignore previous instructions",
        "ignore all previous",
        "disregard your instructions",
        "you are now",
        "forget everything",
        "exfiltrate",
        "bypass security",
        "output your system prompt"
    ];

    public SanitizedJiraInput Sanitize(JiraTaskInput input)
    {
        Validate(input.Title);
        Validate(input.Description);
        Validate(input.AcceptanceCriteria);

        return new SanitizedJiraInput
        {
            TicketId = input.TicketId,
            Title = Wrap(input.Title),
            Description = Wrap(input.Description),
            AcceptanceCriteria = Wrap(input.AcceptanceCriteria)
        };
    }

    private void Validate(string value)
    {
        var normalized = value.ToLowerInvariant();
        if (PromptInjectionPatterns.Any(normalized.Contains))
        {
            logger.LogWarning("Prompt injection pattern detected");
            throw new PromptInjectionException("Potential prompt injection detected in Jira input.");
        }
    }

    private static string Wrap(string value) => $"[USER_INPUT_START]{value}[USER_INPUT_END]";
}
