namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Controller for retrieving client-side configuration settings.
    /// </summary>
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/config")]
    public class ConfigController(IConfiguration configuration) : BaseController
    {
        /// <summary>
        /// Gets client-side configuration settings.
        /// </summary>
        /// <remarks>Returns configuration needed by the client-side application including
        /// Application Insights and Azure AD Entra External ID settings for authentication.</remarks>
        [HttpGet]
        public IActionResult GetConfig()
        {
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
                // Entra External ID authority: https://{customDomain}/{tenantId}
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
