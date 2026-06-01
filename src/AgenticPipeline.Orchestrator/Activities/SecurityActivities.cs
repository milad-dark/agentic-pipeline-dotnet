using AgenticPipeline.Domain.Attributes;
using AgenticPipeline.Domain.Models;
using AgenticPipeline.Security.Services;

namespace AgenticPipeline.Orchestrator.Activities;

[Activity]
public sealed class SecurityActivities(InputSanitizer sanitizer, SecretScanner secretScanner)
{
    public Task<SanitizedJiraInput> SanitizeAndValidateAsync(JiraTaskInput input, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        secretScanner.ScanOrThrow(input.Title);
        secretScanner.ScanOrThrow(input.Description);
        secretScanner.ScanOrThrow(input.AcceptanceCriteria);

        return Task.FromResult(sanitizer.Sanitize(input));
    }
}
