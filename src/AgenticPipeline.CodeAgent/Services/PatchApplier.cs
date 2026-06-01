namespace AgenticPipeline.CodeAgent.Services;

public sealed class PatchApplier
{
    public string Apply(string originalContent, string unifiedDiff)
    {
        if (string.IsNullOrWhiteSpace(unifiedDiff))
        {
            return originalContent;
        }

        var originalLines = originalContent.Split('\n').ToList();
        var diffLines = unifiedDiff.Split('\n');
        var output = new List<string>();
        var sourceIndex = 0;

        foreach (var line in diffLines)
        {
            if (line.StartsWith("@@") || line.StartsWith("---") || line.StartsWith("+++"))
            {
                continue;
            }

            if (line.StartsWith("-"))
            {
                sourceIndex++;
                continue;
            }

            if (line.StartsWith("+"))
            {
                output.Add(line[1..]);
                continue;
            }

            if (sourceIndex < originalLines.Count)
            {
                output.Add(originalLines[sourceIndex]);
                sourceIndex++;
            }
            else
            {
                output.Add(line.TrimStart(' '));
            }
        }

        while (sourceIndex < originalLines.Count)
        {
            output.Add(originalLines[sourceIndex++]);
        }

        return string.Join('\n', output);
    }
}
