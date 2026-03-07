namespace TrashMob.Security
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Authentication filter that validates API key for PRIVO webhook requests.
    /// </summary>
    public class PrivoApiKeyAuthenticationFilter(
        IKeyVaultManager keyVaultManager,
        ILogger<PrivoApiKeyAuthenticationFilter> logger) : IAsyncAuthorizationFilter
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private const string KeyVaultSecretName = "Privo-ApiKey";

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader))
                {
                    context.Result = new JsonResult(new { error = "API key header not found." })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };

                    return Task.CompletedTask;
                }

                var expectedKey = keyVaultManager.GetSecret(KeyVaultSecretName);

                if (!string.Equals(apiKeyHeader, expectedKey, StringComparison.Ordinal))
                {
                    context.Result = new JsonResult(new { error = "Invalid API key." })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };

                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while validating PRIVO API key");
                context.Result = new JsonResult(new { error = "A server error has occurred." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }

            return Task.CompletedTask;
        }
    }
}
