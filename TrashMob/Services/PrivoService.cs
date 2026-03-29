namespace TrashMob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// PRIVO consent management and identity verification HTTP client.
    /// Wraps all 10 PRIVO API sections with token management.
    /// </summary>
    public class PrivoService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMemoryCache memoryCache,
        ILogger<PrivoService> logger) : IPrivoService
    {
        private const string TokenCacheKey = "Privo_AccessToken";
        private const string ServiceIdentifierConfigKey = "Privo:ServiceIdentifier";
        private const string BaseUrlConfigKey = "Privo:BaseUrl";
        private static readonly TimeSpan TokenCacheDuration = TimeSpan.FromMinutes(25);
        private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);

        // PRIVO attribute identifiers
        private const string AttrPrincipalGivenName = "trashmobservice_att_principal_given_name";
        private const string AttrPrincipalFamilyName = "trashmobservice_att_principal_family_name";
        private const string AttrPrincipalEmail = "trashmobservice_att_principal_email";
        private const string AttrPrincipalBirthdate = "trashmobservice_att_principal_birthdate";
        private const string AttrGranterBirthdate = "trashmobservice_att_granter_birthdate";
        private const string AttrGranterEmail = "trashmobservice_att_granter_email";
        private const string AttrGranterGivenName = "trashmobservice_att_granter_given_name";
        private const string AttrGranterFamilyName = "trashmobservice_att_granter_family_name";

        // PRIVO feature identifiers
        private static readonly string[] AllFeatures =
        [
            "trashmobservice_adult_identity_verification",
            "trashmobservice_account",
            "trashmobservice_leaderboard",
            "trashmobservice_social",
            "trashmobservice_newsletter",
            "trashmobservice_notifications",
            "trashmobservice_geolocation",
            "trashmobservice_team",
            "trashmobservice_photo_uploads",
        ];

        private string BaseUrl => configuration[BaseUrlConfigKey] ?? "https://consent-svc-int.privo.com";
        private string ServiceIdentifier => configuration[ServiceIdentifierConfigKey] ?? "trashmobservice";

        /// <inheritdoc />
        public async Task<PrivoConsentResponse> CreateAdultVerificationRequestAsync(
            User user, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return null;

            var payload = new Dictionary<string, object>
            {
                ["service_identifier"] = ServiceIdentifier,
                ["principal"] = new Dictionary<string, object>
                {
                    ["ext_id"] = user.Id.ToString(),
                    ["attributes"] = new[]
                    {
                        new { identifier = AttrGranterGivenName, value = user.GivenName ?? string.Empty },
                        new { identifier = AttrGranterFamilyName, value = user.Surname ?? string.Empty },
                        new { identifier = AttrGranterEmail, value = user.Email ?? string.Empty },
                        new { identifier = AttrGranterBirthdate, value = user.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty },
                    },
                },
            };

            return await PostConsentRequestAsync(token, payload, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> GetDirectVerificationUrlAsync(
            string consentIdentifier, string redirectUrl, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return null;

            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var url = $"{BaseUrl}/s2s/api/v1.0/{ServiceIdentifier}/consents/{consentIdentifier}/verification/session?redirect_url={Uri.EscapeDataString(redirectUrl)}";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("PRIVO GetDirectVerificationUrl returned {StatusCode} for consent {ConsentId}",
                        response.StatusCode, consentIdentifier);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get direct verification URL for consent {ConsentId}", consentIdentifier);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<PrivoUserInfo> GetUserInfoBySidAsync(
            string sid, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return null;

            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var url = $"{BaseUrl}/s2s/api/v1.0/{ServiceIdentifier}/accounts/sid/{sid}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("PRIVO GetUserInfo returned {StatusCode} for SiD {Sid}",
                        response.StatusCode, sid);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return ParseUserInfo(json, sid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get user info from PRIVO for SiD {Sid}", sid);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<PrivoConsentResponse> CreateParentInitiatedChildConsentAsync(
            User parent, Dependent child, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return null;

            var principal = new Dictionary<string, object>
            {
                ["ext_id"] = child.Id.ToString(),
                ["attributes"] = new[]
                {
                    new { identifier = AttrPrincipalGivenName, value = child.FirstName ?? string.Empty },
                    new { identifier = AttrPrincipalFamilyName, value = child.LastName ?? string.Empty },
                    new { identifier = AttrPrincipalBirthdate, value = child.DateOfBirth.ToString("yyyy-MM-dd") },
                },
            };

            var payload = new Dictionary<string, object>
            {
                ["service_identifier"] = ServiceIdentifier,
                ["principal"] = principal,
                ["granter_email"] = parent.Email ?? string.Empty,
            };

            // If parent has a PRIVO SiD, use it for the granter reference
            if (!string.IsNullOrEmpty(parent.PrivoSid))
            {
                payload["granter_sid"] = parent.PrivoSid;
            }

            return await PostConsentRequestAsync(token, payload, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PrivoConsentResponse> CreateChildInitiatedConsentAsync(
            string childFirstName, string childEmail, DateOnly childBirthDate,
            string parentEmail, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return null;

            var payload = new Dictionary<string, object>
            {
                ["service_identifier"] = ServiceIdentifier,
                ["principal"] = new Dictionary<string, object>
                {
                    ["attributes"] = new[]
                    {
                        new { identifier = AttrPrincipalGivenName, value = childFirstName },
                        new { identifier = AttrPrincipalEmail, value = childEmail },
                        new { identifier = AttrPrincipalBirthdate, value = childBirthDate.ToString("yyyy-MM-dd") },
                    },
                },
                ["granter_email"] = parentEmail,
            };

            return await PostConsentRequestAsync(token, payload, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> GetConsentStatusAsync(
            string consentIdentifier, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return null;

            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var url = $"{BaseUrl}/s2s/api/v1.0/{ServiceIdentifier}/consents/{consentIdentifier}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("PRIVO GetConsentStatus returned {StatusCode} for consent {ConsentId}",
                        response.StatusCode, consentIdentifier);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.TryGetProperty("state", out var stateProp) ? stateProp.GetString() : null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get consent status from PRIVO for consent {ConsentId}", consentIdentifier);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task SyncEmailVerificationAsync(
            string sid, string attributeIdentifier, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return;

            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var url = $"{BaseUrl}/s2s/api/v1.0/{ServiceIdentifier}/accounts/sid/{sid}/attributes/{attributeIdentifier}/ial";

                var payload = new { ial = 2 }; // IAL value TBD by PRIVO — using 2 as placeholder
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("PRIVO SyncEmailVerification returned {StatusCode} for SiD {Sid}",
                        response.StatusCode, sid);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to sync email verification to PRIVO for SiD {Sid}", sid);
            }
        }

        /// <inheritdoc />
        public async Task RevokeConsentAsync(
            string principalSid, string granterSid, CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (token == null) return;

            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var url = $"{BaseUrl}/s2s/api/v1.0/{ServiceIdentifier}/accounts/sid/{principalSid}/{granterSid}/features/revoke";

                var payload = new { features = AllFeatures };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("PRIVO RevokeConsent returned {StatusCode} for principal {PrincipalSid}, granter {GranterSid}",
                        response.StatusCode, principalSid, granterSid);
                }
                else
                {
                    logger.LogInformation("PRIVO consent revoked for principal {PrincipalSid}, granter {GranterSid}",
                        principalSid, granterSid);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to revoke consent in PRIVO for principal {PrincipalSid}", principalSid);
            }
        }

        #region Token Management

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (memoryCache.TryGetValue(TokenCacheKey, out string cachedToken))
            {
                return cachedToken;
            }

            await TokenSemaphore.WaitAsync(cancellationToken);
            try
            {
                // Double-check after acquiring lock
                if (memoryCache.TryGetValue(TokenCacheKey, out cachedToken))
                {
                    return cachedToken;
                }

                return await AcquireTokenAsync(cancellationToken);
            }
            finally
            {
                TokenSemaphore.Release();
            }
        }

        private async Task<string> AcquireTokenAsync(CancellationToken cancellationToken)
        {
            var clientId = configuration["Privo-ClientId"] ?? configuration["Privo:ClientId"];
            var clientSecret = configuration["Privo-ClientSecret"] ?? configuration["Privo:ClientSecret"];

            logger.LogInformation(
                "PRIVO token acquisition: BaseUrl={BaseUrl}, ClientId={ClientIdPresent}, ClientSecret={ClientSecretPresent}",
                BaseUrl,
                string.IsNullOrWhiteSpace(clientId) ? "MISSING" : clientId[..4] + "...",
                string.IsNullOrWhiteSpace(clientSecret) ? "MISSING" : "***set***");

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                logger.LogWarning("PRIVO service disabled: Privo-ClientId or Privo-ClientSecret not configured");
                return null;
            }

            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var tokenUrl = $"{BaseUrl}/token";

                logger.LogInformation("PRIVO token request: POST {TokenUrl}", tokenUrl);

                var formData = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("scope", "openid"),
                ]);

                var response = await client.PostAsync(tokenUrl, formData, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(
                        "PRIVO token request failed: {StatusCode} {ReasonPhrase}, URL={TokenUrl}, Response={ResponseBody}",
                        (int)response.StatusCode, response.ReasonPhrase, tokenUrl, responseBody);
                    return null;
                }

                logger.LogInformation("PRIVO token response: {StatusCode}, BodyLength={BodyLength}",
                    (int)response.StatusCode, responseBody.Length);

                using var doc = JsonDocument.Parse(responseBody);

                if (!doc.RootElement.TryGetProperty("access_token", out var tokenProp))
                {
                    logger.LogWarning("PRIVO token response missing access_token. Keys={Keys}",
                        string.Join(", ", doc.RootElement.EnumerateObject().Select(p => p.Name)));
                    return null;
                }

                var accessToken = tokenProp.GetString();
                memoryCache.Set(TokenCacheKey, accessToken, TokenCacheDuration);

                logger.LogInformation("PRIVO access token acquired successfully");
                return accessToken;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to acquire PRIVO access token from {BaseUrl}/token", BaseUrl);
                return null;
            }
        }

        #endregion

        #region Helpers

        private async Task<PrivoConsentResponse> PostConsentRequestAsync(
            string token, Dictionary<string, object> payload, CancellationToken cancellationToken)
        {
            try
            {
                var client = httpClientFactory.CreateClient("Privo");
                var url = $"{BaseUrl}/s2s/api/v1.0/{ServiceIdentifier}/requests";

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogWarning("PRIVO consent request returned {StatusCode}: {ErrorBody}",
                        response.StatusCode, errorBody);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return ParseConsentResponse(json);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create PRIVO consent request");
                return null;
            }
        }

        private static PrivoConsentResponse ParseConsentResponse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var result = new PrivoConsentResponse();

            if (root.TryGetProperty("sid", out var sidProp))
                result.Sid = sidProp.GetString() ?? string.Empty;

            if (root.TryGetProperty("consent_identifier", out var consentIdProp))
                result.ConsentIdentifier = consentIdProp.GetString() ?? string.Empty;

            if (root.TryGetProperty("consent_url", out var consentUrlProp))
                result.ConsentUrl = consentUrlProp.GetString() ?? string.Empty;

            if (root.TryGetProperty("granter_sid", out var granterSidProp))
                result.GranterSid = granterSidProp.GetString();

            return result;
        }

        private static PrivoUserInfo ParseUserInfo(string json, string sid)
        {
            var result = new PrivoUserInfo { Sid = sid };

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("features", out var featuresProp) && featuresProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var feature in featuresProp.EnumerateArray())
                {
                    if (feature.TryGetProperty("identifier", out var idProp) &&
                        feature.TryGetProperty("state", out var stateProp))
                    {
                        result.Features[idProp.GetString() ?? string.Empty] = stateProp.GetString() ?? string.Empty;
                    }
                }
            }

            if (root.TryGetProperty("attributes", out var attrsProp) && attrsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var attr in attrsProp.EnumerateArray())
                {
                    if (attr.TryGetProperty("identifier", out var idProp) &&
                        attr.TryGetProperty("value", out var valProp))
                    {
                        result.Attributes[idProp.GetString() ?? string.Empty] = valProp.GetString() ?? string.Empty;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
