namespace TrashMob.Shared.Managers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Core;
    using Azure.Identity;
    using AzureMapsToolkit.Search;
    using AzureMapsToolkit.Spatial;
    using AzureMapsToolkit.Timezone;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Provides map-related services including distance calculations, timezone lookups, and address resolution using Azure Maps.
    /// </summary>
    public class MapManager : IMapManager
    {
        private const string AzureMapKeyName = "AzureMapsKey";
        private const string AzureMapsClientIdName = "AzureMapsClientId";
        private const string GoogleMapKeyName = "GoogleMapsKey";
        private const string AzureMapsScope = "https://atlas.microsoft.com/.default";

        private const int MetersPerKilometer = 1000;
        private const int MetersPerMile = 1609;
        private readonly IConfiguration configuration;
        private readonly ILogger<MapManager> logger;
        private readonly TokenCredential tokenCredential;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapManager"/> class.
        /// </summary>
        /// <param name="configuration">The configuration containing map API keys.</param>
        /// <param name="logger">The logger instance.</param>
        public MapManager(IConfiguration configuration, ILogger<MapManager> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.tokenCredential = new DefaultAzureCredential();
        }

        /// <summary>
        /// Gets the Azure Maps Client ID for managed identity authentication.
        /// </summary>
        /// <returns>The Azure Maps Client ID, or null if not configured.</returns>
        private string GetAzureMapsClientId()
        {
            return configuration[AzureMapsClientIdName];
        }

        /// <summary>
        /// Determines if managed identity authentication should be used.
        /// Managed identity is used when AzureMapsClientId is configured.
        /// </summary>
        private bool UseManagedIdentity => !string.IsNullOrEmpty(GetAzureMapsClientId());

        /// <summary>
        /// Gets a bearer token for Azure Maps API authentication using managed identity.
        /// </summary>
        private async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken = default)
        {
            var tokenRequestContext = new TokenRequestContext(new[] { AzureMapsScope });
            var token = await tokenCredential.GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);
            return token.Token;
        }

        /// <inheritdoc />
        public string GetMapKey()
        {
            return configuration[AzureMapKeyName];
        }

        /// <inheritdoc />
        public string GetGoogleMapKey()
        {
            return configuration[GoogleMapKeyName];
        }

        /// <inheritdoc />
        public async Task<double> GetDistanceBetweenTwoPointsAsync(Tuple<double, double> pointA,
            Tuple<double, double> pointB, bool IsMetric = true)
        {
            var azureMaps = new AzureMapsToolkit.AzureMapsServices(GetMapKey());
            var distanceRequest = new GreatCircleDistanceRequest
            {
                Query = $"{pointA.Item1},{pointA.Item2}:{pointB.Item1},{pointB.Item2}",
                Start = new Coordinate { Lat = pointA.Item1, Lon = pointA.Item2 },
                End = new Coordinate { Lat = pointB.Item1, Lon = pointB.Item2 },
            };

            logger.LogInformation("Getting distance between two points: {0}",
                JsonSerializer.Serialize(distanceRequest));

            var response = await azureMaps.GetGreatCircleDistance(distanceRequest).ConfigureAwait(false);

            logger.LogInformation("Response from getting distance between two points: {0}",
                JsonSerializer.Serialize(response));

            try
            {
                if (response.HttpResponseCode != (int)HttpStatusCode.OK && response.HttpResponseCode != 0)
                {
                    throw new Exception($"Error getting GetGreatCircleDistance: {JsonSerializer.Serialize(response)}");
                }

                var distanceInMeters = (long)response.Result.Result.DistanceInMeters;

                logger.LogInformation("Distance in Meters: {0}", distanceInMeters);

                if (IsMetric)
                {
                    var res = distanceInMeters / MetersPerKilometer;
                    logger.LogInformation("Kilometers : {0}", res);
                    return res;
                }
                else
                {
                    var res = distanceInMeters / MetersPerMile;
                    logger.LogInformation("Miles : {0}", res);
                    return res;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception encountered");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetTimeForPointAsync(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset)
        {
            var azureMaps = new AzureMapsToolkit.AzureMapsServices(GetMapKey());

            if (azureMaps == null)
            {
                logger.LogError("Failed to get instance of azuremaps.");
                throw new Exception("Failed to get instance of azuremaps");
            }

            var timezoneRequest = new TimeZoneRequest
            {
                Query = $"{pointA.Item1},{pointA.Item2}",
                TimeStamp = dateTimeOffset.UtcDateTime.ToString("O"),
            };

            logger.LogInformation("Getting time for timezoneRequest: {0}", JsonSerializer.Serialize(timezoneRequest));

            var response = await azureMaps.GetTimezoneByCoordinates(timezoneRequest).ConfigureAwait(false);

            logger.LogInformation("Response from getting time for timezoneRequest: {0}",
                JsonSerializer.Serialize(response));

            if (response.HttpResponseCode != (int)HttpStatusCode.OK && response.HttpResponseCode != 0)
            {
                throw new Exception($"Error getting timezonebycoordinates: {response}");
            }

            return response?.Result?.TimeZones[0]?.ReferenceTime?.WallTime;
        }

        /// <inheritdoc />
        public async Task<Address> GetAddressAsync(double latitude, double longitude)
        {
            var azureMaps = new AzureMapsToolkit.AzureMapsServices(GetMapKey());

            if (azureMaps == null)
            {
                logger.LogError("Failed to get instance of azuremaps.");
                throw new Exception("Failed to get instance of azuremaps");
            }

            var searchAddressReverseRequest = new SearchAddressReverseRequest();
            searchAddressReverseRequest.Query = $"{latitude},{longitude}";

            logger.LogInformation("Getting address for searchAddressReverseRequest: {0}",
                JsonSerializer.Serialize(searchAddressReverseRequest));

            var response = await azureMaps.GetSearchAddressReverse(searchAddressReverseRequest).ConfigureAwait(false);

            logger.LogInformation("Response from getting address for searcAddressReverseRequest: {0}",
                JsonSerializer.Serialize(response));

            if (response.HttpResponseCode != (int)HttpStatusCode.OK && response.HttpResponseCode != 0)
            {
                throw new Exception($"Error getting address: {response}");
            }

            var address = new Address
            {
                StreetAddress = response?.Result?.Addresses[0].Address.StreetNameAndNumber,
                City = response?.Result?.Addresses[0].Address.Municipality,
                Country = response?.Result?.Addresses[0].Address.Country,
                Region = response?.Result?.Addresses[0].Address.CountrySubdivisionName,
                PostalCode = response?.Result?.Addresses[0].Address.PostalCode,
                County = response?.Result?.Addresses[0].Address.CountrySecondarySubdivision,
            };

            return address;
        }

        /// <inheritdoc />
        public async Task<string> SearchAddressAsync(string query, string entityType = null)
        {
            logger.LogInformation("Searching address with query: {Query}, UseManagedIdentity: {UseManagedIdentity}", query, UseManagedIdentity);

            using var httpClient = new HttpClient();
            string url;

            if (UseManagedIdentity)
            {
                // Use managed identity with bearer token authentication
                url = $"https://atlas.microsoft.com/search/address/json?typeahead=true&api-version=1.0&query={Uri.EscapeDataString(query)}";

                var token = await GetBearerTokenAsync().ConfigureAwait(false);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Add("x-ms-client-id", GetAzureMapsClientId());
            }
            else
            {
                // Fall back to subscription key authentication for local development
                var subscriptionKey = GetMapKey();
                url = $"https://atlas.microsoft.com/search/address/json?typeahead=true&subscription-key={subscriptionKey}&api-version=1.0&query={Uri.EscapeDataString(query)}";
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                url += $"&entityType={Uri.EscapeDataString(entityType)}";
            }

            var response = await httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Azure Maps search failed with status code: {StatusCode}", response.StatusCode);
                throw new Exception($"Azure Maps search failed: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            logger.LogInformation("Address search completed with {Length} bytes", content.Length);

            return content;
        }

        /// <inheritdoc />
        public async Task<string> ReverseGeocodeAsync(double latitude, double longitude)
        {
            logger.LogInformation("Reverse geocoding for coordinates: {Lat}, {Lon}, UseManagedIdentity: {UseManagedIdentity}", latitude, longitude, UseManagedIdentity);

            using var httpClient = new HttpClient();
            string url;

            if (UseManagedIdentity)
            {
                // Use managed identity with bearer token authentication
                url = $"https://atlas.microsoft.com/search/address/reverse/json?api-version=1.0&query={latitude},{longitude}";

                var token = await GetBearerTokenAsync().ConfigureAwait(false);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Add("x-ms-client-id", GetAzureMapsClientId());
            }
            else
            {
                // Fall back to subscription key authentication for local development
                var subscriptionKey = GetMapKey();
                url = $"https://atlas.microsoft.com/search/address/reverse/json?subscription-key={subscriptionKey}&api-version=1.0&query={latitude},{longitude}";
            }

            var response = await httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Azure Maps reverse geocode failed with status code: {StatusCode}", response.StatusCode);
                throw new Exception($"Azure Maps reverse geocode failed: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            logger.LogInformation("Reverse geocode completed with {Length} bytes", content.Length);

            return content;
        }
    }
}