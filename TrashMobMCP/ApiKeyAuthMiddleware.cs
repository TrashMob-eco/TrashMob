namespace TrashMobMCP;

/// <summary>
/// Middleware that validates an API key from the Authorization header.
/// The expected key is read from the MCP_API_KEY environment variable.
/// </summary>
public class ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger)
{
    private const string ApiKeyHeaderName = "Authorization";
    private const string BearerPrefix = "Bearer ";

    public async Task InvokeAsync(HttpContext context)
    {
        var expectedKey = Environment.GetEnvironmentVariable("MCP_API_KEY");

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            logger.LogWarning("MCP_API_KEY environment variable is not set. All requests will be rejected.");
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Server is not configured for authentication.");
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var headerValue))
        {
            logger.LogWarning("Request rejected: missing Authorization header");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing Authorization header.");
            return;
        }

        var providedKey = headerValue.ToString();

        // Support both "Bearer <key>" and raw "<key>" formats
        if (providedKey.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            providedKey = providedKey[BearerPrefix.Length..];
        }

        if (!string.Equals(providedKey, expectedKey, StringComparison.Ordinal))
        {
            logger.LogWarning("Request rejected: invalid API key");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key.");
            return;
        }

        await next(context);
    }
}
