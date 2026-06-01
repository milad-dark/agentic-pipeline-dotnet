using System.Text.RegularExpressions;
using AgenticPipeline.Security.Exceptions;

namespace AgenticPipeline.Security.Services;

public sealed class SecretScanner
{
    private static readonly Regex[] SecretPatterns =
    [
        new("(api[_-]?key|secret)[\\s:=]+[A-Za-z0-9_\\-]{12,}", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new("(host|server)=.+;(database|db)=.+;(user id|uid|username)=.+;(password|pwd)=.+", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new("[A-Za-z0-9+/]{64,}={0,2}", RegexOptions.Compiled)
    ];

    public void ScanOrThrow(string text)
    {
        if (SecretPatterns.Any(p => p.IsMatch(text)))
        {
            throw new SecretLeakageException("Potential secret leakage pattern detected.");
        }
    }
}
