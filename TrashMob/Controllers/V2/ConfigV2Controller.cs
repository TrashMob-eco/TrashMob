namespace TrashMob.Controllers.V2
{
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// V2 controller for retrieving client-side configuration settings.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/config")]
    public class ConfigV2Controller(
        IConfiguration configuration,
        ILogger<ConfigV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets client-side configuration settings including Application Insights
        /// and Azure AD Entra External ID settings for authentication.
        /// </summary>
        /// <returns>Configuration object with instrumentation key and Entra settings.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetConfig()
        {
            logger.LogInformation("V2 GetConfig");

            var connectionString = configuration["ApplicationInsights:ConnectionString"];

            // Extract instrumentation key from connection string
            // Format: InstrumentationKey=xxx;IngestionEndpoint=xxx;LiveEndpoint=xxx
            string instrumentationKey = null;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                foreach (var part in connectionString.Split(';'))
                {
                    if (part.StartsWith("InstrumentationKey="))
                    {
                        instrumentationKey = part.Substring("InstrumentationKey=".Length);
                        break;
                    }
                }
            }

            var entraInstance = configuration["AzureAdEntra:Instance"]?.TrimEnd('/');
            var entraDomain = configuration["AzureAdEntra:Domain"];
            var entraFrontendClientId = configuration["AzureAdEntra:FrontendClientId"];
            var entraTenantId = configuration["AzureAdEntra:TenantId"];

            string authority = null;
            string authorityDomain = null;

            if (!string.IsNullOrWhiteSpace(entraInstance) && !string.IsNullOrWhiteSpace(entraTenantId))
            {
                // Entra External ID authority: https://{tenant}.ciamlogin.com/{tenantId}
                authority = $"{entraInstance}/{entraTenantId}";

                if (entraInstance.StartsWith("https://"))
                {
                    authorityDomain = entraInstance.Substring("https://".Length);
                }
            }

            return Ok(new
            {
                applicationInsightsKey = instrumentationKey,
                authProvider = "entra",
                azureAdEntra = new
                {
                    clientId = entraFrontendClientId,
                    authorityDomain,
                    authority,
                    scopes = entraDomain is not null
                        ? new[]
                        {
                            $"https://{entraDomain}/api/TrashMob.Read",
                            $"https://{entraDomain}/api/TrashMob.Writes",
                            "email",
                        }
                        : null,
                },
            });
        }
    }
}
