namespace TrashMobMobile.Services
{
    using System.Net;
    using System.Text.Json;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// REST service implementation for PRIVO consent management API endpoints.
    /// </summary>
    public class PrivoConsentRestService(IHttpClientFactory httpClientFactory)
        : RestServiceBase(httpClientFactory), IPrivoConsentRestService
    {
        protected override string Controller => "privo/";

        /// <inheritdoc />
        public async Task<ParentalConsentDto> InitiateAdultVerificationAsync(CancellationToken cancellationToken)
        {
            using var response = await AuthorizedHttpClient.PostAsync("verify", null, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<ParentalConsentDto>(content, SerializerOptions)!;
        }

        /// <inheritdoc />
        public async Task<ParentalConsentDto> InitiateChildConsentAsync(
            Guid dependentId, CancellationToken cancellationToken)
        {
            var requestUri = $"consent/child/{dependentId}";
            using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<ParentalConsentDto>(content, SerializerOptions)!;
        }

        /// <inheritdoc />
        public async Task<ParentalConsentDto?> GetVerificationStatusAsync(CancellationToken cancellationToken)
        {
            using var response = await AuthorizedHttpClient.GetAsync("status", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<ParentalConsentDto>(content, SerializerOptions);
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>?> GetMinorPermissionsAsync(CancellationToken cancellationToken)
        {
            using var response = await AuthorizedHttpClient.GetAsync("permissions", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(content, SerializerOptions);
        }

        /// <inheritdoc />
        public async Task RevokeConsentAsync(Guid consentId, string reason, CancellationToken cancellationToken)
        {
            var requestUri = $"consent/{consentId}/revoke?reason={Uri.EscapeDataString(reason)}";
            using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
