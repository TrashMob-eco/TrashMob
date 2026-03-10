namespace TrashMobMobile.Services
{
    using System.Diagnostics;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models.Poco;

    public class EventAttendeeRouteRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventAttendeeRouteRestService
    {
        protected override string Controller => "eventattendeeroutes";

        public async Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesAsync(Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + eventId + "/" + userId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<DisplayEventAttendeeRoute>>(content) ?? [];
            }
        }

        public async Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesForEventAsync(Guid eventId,
                CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/by-event/" + eventId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<DisplayEventAttendeeRoute>>(content) ?? [];
            }
        }

        public async Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesForUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/by-user/" + userId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<DisplayEventAttendeeRoute>>(content) ?? [];
            }
        }

        public async Task<DisplayEventAttendeeRoute> GetEventAttendeeRouteAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + id;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(content)!;
            }
        }

        public async Task<DisplayEventAttendeeRoute> AddEventAttendeeRouteAsync(DisplayEventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(eventAttendeeRoute, typeof(DisplayEventAttendeeRoute), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(responseContent)!;
            }
        }

        public async Task DeleteEventAttendeeRouteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var requestUri = string.Concat(Controller, $"/{id}");

            using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<DisplayEventAttendeeRoute> SimulateRouteAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("events/");
            var requestUri = eventId + "/routes/simulate";

            using (var response = await client.PostAsync(requestUri, null, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(content)!;
            }
        }

        public async Task<DisplayEventAttendeeRoute> UpdateRouteMetadataAsync(Guid routeId, UpdateRouteMetadataRequest request, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + routeId;
            var content = JsonContent.Create(request, typeof(UpdateRouteMetadataRequest), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PutAsync(requestUri, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(responseContent)!;
            }
        }

        public async Task<EventSummaryPrefill> GetEventSummaryPrefillAsync(Guid eventId, int weightUnitId = 1, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("events/");
            var requestUri = $"{eventId}/routes/summary-prefill?weightUnitId={weightUnitId}";

            using (var response = await client.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<EventSummaryPrefill>(content)!;
            }
        }

        public async Task<DisplayEventAttendeeRoute> TrimRouteTimeAsync(Guid routeId, TrimRouteTimeRequest request, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + routeId + "/trim-time";
            var content = JsonContent.Create(request, typeof(TrimRouteTimeRequest), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PutAsync(requestUri, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(responseContent)!;
            }
        }

        public async Task<DisplayEventAttendeeRoute> RestoreRouteTimeAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + routeId + "/restore-time";

            using (var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(responseContent)!;
            }
        }
    }
}
