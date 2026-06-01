using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.ReviewerAgent.Services;

public sealed class ArchitectureGuard
{
    public ArchitectureViolations Check(IReadOnlyList<CodePatch> patches)
    {
        var violations = new ArchitectureViolations();

        foreach (var patch in patches)
        {
            var diff = patch.UnifiedDiff;

            if (patch.TargetFile.Contains("Application", StringComparison.OrdinalIgnoreCase) &&
                diff.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase))
            {
                violations.Items.Add($"Application layer referencing Infrastructure in {patch.TargetFile}");
            }

            if (!patch.TargetFile.Contains("Repository", StringComparison.OrdinalIgnoreCase) &&
                diff.Contains("DbContext", StringComparison.OrdinalIgnoreCase))
            {
                violations.Items.Add($"DbContext usage outside repository in {patch.TargetFile}");
            }

            if (diff.Contains(".Result", StringComparison.Ordinal) || diff.Contains(".Wait()", StringComparison.Ordinal))
            {
                violations.Items.Add($"Blocking async call detected in {patch.TargetFile}");
            }
        }

        return violations;
    }
}
