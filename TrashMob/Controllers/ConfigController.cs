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
            if (!string.IsNullOrEmpty(connectionString))
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

            // Azure AD B2C settings for frontend MSAL authentication
            // These are public configuration values, not secrets
            var b2cInstance = configuration["AzureAdB2C:Instance"]?.TrimEnd('/');
            var b2cDomain = configuration["AzureAdB2C:Domain"];
            var b2cFrontendClientId = configuration["AzureAdB2C:FrontendClientId"];
            var b2cSignUpSignInPolicyId = configuration["AzureAdB2C:SignUpSignInPolicyId"];

            // Build B2C authority URLs for MSAL
            // Format: https://{tenant}.b2clogin.com/{tenant}.onmicrosoft.com/{policy}
            string signUpSignInAuthority = null;
            string deleteUserAuthority = null;
            string profileEditAuthority = null;
            string authorityDomain = null;

            if (!string.IsNullOrEmpty(b2cInstance) && !string.IsNullOrEmpty(b2cDomain))
            {
                var baseAuthority = $"{b2cInstance}/{b2cDomain}";
                signUpSignInAuthority = $"{baseAuthority}/{b2cSignUpSignInPolicyId}";
                deleteUserAuthority = $"{baseAuthority}/B2C_1A_TM_DEREGISTER";
                profileEditAuthority = $"{baseAuthority}/B2C_1A_TM_PROFILEEDIT";

                // Extract authority domain from instance (e.g., "trashmob.b2clogin.com" from "https://trashmob.b2clogin.com")
                if (b2cInstance.StartsWith("https://"))
                {
                    authorityDomain = b2cInstance.Substring("https://".Length);
                }
            }

            return Ok(new
            {
                applicationInsightsKey = instrumentationKey,
                azureAdB2C = new
                {
                    clientId = b2cFrontendClientId,
                    authorityDomain = authorityDomain,
                    policies = new
                    {
                        signUpSignIn = b2cSignUpSignInPolicyId,
                        deleteUser = "B2C_1A_TM_DEREGISTER",
                        profileEdit = "B2C_1A_TM_PROFILEEDIT"
                    },
                    authorities = new
                    {
                        signUpSignIn = signUpSignInAuthority,
                        deleteUser = deleteUserAuthority,
                        profileEdit = profileEditAuthority
                    },
                    scopes = b2cDomain != null ? new[]
                    {
                        $"https://{b2cDomain}/api/TrashMob.Read",
                        $"https://{b2cDomain}/api/TrashMob.Writes",
                        "email"
                    } : null
                }
            });
        }
    }
}
