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
        /// Application Insights and Azure AD B2C settings for authentication.</remarks>
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

            var useEntraExternalId = configuration.GetValue<bool>("UseEntraExternalId");

            if (useEntraExternalId)
            {
                return Ok(BuildEntraConfig(instrumentationKey));
            }

            return Ok(BuildB2CConfig(instrumentationKey));
        }

        private object BuildB2CConfig(string instrumentationKey)
        {
            var b2cInstance = configuration["AzureAdB2C:Instance"]?.TrimEnd('/');
            var b2cDomain = configuration["AzureAdB2C:Domain"];
            var b2cFrontendClientId = configuration["AzureAdB2C:FrontendClientId"];
            var b2cSignUpSignInPolicyId = configuration["AzureAdB2C:SignUpSignInPolicyId"];

            string signUpSignInAuthority = null;
            string deleteUserAuthority = null;
            string profileEditAuthority = null;
            string authorityDomain = null;

            if (!string.IsNullOrWhiteSpace(b2cInstance) && !string.IsNullOrWhiteSpace(b2cDomain))
            {
                var baseAuthority = $"{b2cInstance}/{b2cDomain}";
                signUpSignInAuthority = $"{baseAuthority}/{b2cSignUpSignInPolicyId}";
                deleteUserAuthority = $"{baseAuthority}/B2C_1A_TM_DEREGISTER";
                profileEditAuthority = $"{baseAuthority}/B2C_1A_TM_PROFILEEDIT";

                if (b2cInstance.StartsWith("https://"))
                {
                    authorityDomain = b2cInstance.Substring("https://".Length);
                }
            }

            return new
            {
                applicationInsightsKey = instrumentationKey,
                authProvider = "b2c",
                azureAdB2C = new
                {
                    clientId = b2cFrontendClientId,
                    authorityDomain,
                    policies = new
                    {
                        signUpSignIn = b2cSignUpSignInPolicyId,
                        deleteUser = "B2C_1A_TM_DEREGISTER",
                        profileEdit = "B2C_1A_TM_PROFILEEDIT",
                    },
                    authorities = new
                    {
                        signUpSignIn = signUpSignInAuthority,
                        deleteUser = deleteUserAuthority,
                        profileEdit = profileEditAuthority,
                    },
                    scopes = b2cDomain != null
                        ? new[]
                        {
                            $"https://{b2cDomain}/api/TrashMob.Read",
                            $"https://{b2cDomain}/api/TrashMob.Writes",
                            "email",
                        }
                        : null,
                },
            };
        }

        private object BuildEntraConfig(string instrumentationKey)
        {
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

            return new
            {
                applicationInsightsKey = instrumentationKey,
                authProvider = "entra",
                azureAdEntra = new
                {
                    clientId = entraFrontendClientId,
                    authorityDomain,
                    authority,
                    scopes = entraDomain != null
                        ? new[]
                        {
                            $"https://{entraDomain}/api/TrashMob.Read",
                            $"https://{entraDomain}/api/TrashMob.Writes",
                            "email",
                        }
                        : null,
                },
            };
        }
    }
}
