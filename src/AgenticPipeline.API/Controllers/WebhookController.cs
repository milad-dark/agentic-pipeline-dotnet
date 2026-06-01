using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace AgenticPipeline.API.Controllers;

[ApiController]
[Route("api/webhook/github")]
public sealed class WebhookController(IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        var signature = Request.Headers["X-Hub-Signature-256"].ToString();
        if (!await IsValidSignatureAsync(signature, ct))
        {
            return Unauthorized();
        }

        var eventName = Request.Headers["X-GitHub-Event"].ToString();
        if (eventName is not ("pull_request" or "check_run"))
        {
            return Ok(new { handled = false });
        }

        return Ok(new { handled = true, eventName });
    }

    private async Task<bool> IsValidSignatureAsync(string headerSignature, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(headerSignature))
        {
            return false;
        }

        var secret = configuration["GitHub:WebhookSecret"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(secret))
        {
            return false;
        }

        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var computed = "sha256=" + Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(body))).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(headerSignature));
    }
}
