using Microsoft.Extensions.Configuration;

namespace AgenticPipeline.GitHubIntegration.Services;

public sealed class GitHubAppAuthService(IConfiguration configuration)
{
    public Task<string> GetInstallationTokenAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var appId = configuration["GitHub:AppId"] ?? string.Empty;
        var privateKeyPath = configuration["GitHub:PrivateKeyPath"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(privateKeyPath) || !File.Exists(privateKeyPath))
        {
            return Task.FromResult(string.Empty);
        }

        return Task.FromResult(Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
    }
}
