namespace TrashMob.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Queries user information from the CIAM directory via Microsoft Graph API.
    /// Uses client credentials flow (app-only) to read user profiles.
    /// Requires AzureAdEntra:ClientSecret and Microsoft Graph User.Read.All permission.
    /// </summary>
    public class CiamGraphService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<CiamGraphService> logger) : ICiamGraphService
    {
        private ClientSecretCredential credential;
        private bool initialized;
        private bool disabled;

        public async Task<string> GetUserEmailAsync(Guid objectId, CancellationToken cancellationToken = default)
        {
            EnsureInitialized();

            if (disabled)
            {
                return null;
            }

            try
            {
                var token = await credential.GetTokenAsync(
                    new Azure.Core.TokenRequestContext(["https://graph.microsoft.com/.default"]),
                    cancellationToken);

                var client = httpClientFactory.CreateClient("CiamGraph");
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"https://graph.microsoft.com/v1.0/users/{objectId}?$select=mail,otherMails,identities");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Graph API call for user {ObjectId} returned {StatusCode}",
                        objectId, response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return ExtractEmail(json);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch user email from CIAM Graph API for ObjectId {ObjectId}", objectId);
                return null;
            }
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            var tenantId = configuration["AzureAdEntra:TenantId"];
            var clientId = configuration["AzureAdEntra:ClientId"];
            var clientSecret = configuration["AzureAdEntra:ClientSecret"];

            if (string.IsNullOrWhiteSpace(tenantId) ||
                string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret))
            {
                logger.LogWarning("CiamGraphService disabled: AzureAdEntra:ClientSecret not configured");
                disabled = true;
                return;
            }

            credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        }

        private static string ExtractEmail(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Try 'mail' property first
            if (root.TryGetProperty("mail", out var mailProp) &&
                mailProp.ValueKind == JsonValueKind.String)
            {
                var mail = mailProp.GetString();
                if (!string.IsNullOrWhiteSpace(mail))
                {
                    return mail;
                }
            }

            // Try 'otherMails' array
            if (root.TryGetProperty("otherMails", out var otherMailsProp) &&
                otherMailsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in otherMailsProp.EnumerateArray())
                {
                    var email = item.GetString();
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        return email;
                    }
                }
            }

            // Try 'identities' array — CIAM stores sign-up email here for email+password users
            if (root.TryGetProperty("identities", out var identitiesProp) &&
                identitiesProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var identity in identitiesProp.EnumerateArray())
                {
                    if (identity.TryGetProperty("signInType", out var signInType) &&
                        signInType.GetString() == "emailAddress" &&
                        identity.TryGetProperty("issuerAssignedId", out var issuerId))
                    {
                        var email = issuerId.GetString();
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            return email;
                        }
                    }
                }
            }

            return null;
        }
    }
}
