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
                return JsonConvert.DeserializeObject<IEnumerable<DisplayEventAttendeeRoute>>(content);
            }
        }

        public async Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesForEventAsync(Guid eventId,
                CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/byeventid/" + eventId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<DisplayEventAttendeeRoute>>(content);
            }
        }

        public async Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesForUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/byuserid/" + userId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<DisplayEventAttendeeRoute>>(content);
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
                return JsonConvert.DeserializeObject<DisplayEventAttendeeRoute>(content);
            }
        }

        public async Task<DisplayEventAttendeeRoute> AddEventAttendeeRouteAsync(DisplayEventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(eventAttendeeRoute, typeof(DisplayEventAttendeeRoute), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }

            return await GetEventAttendeeRouteAsync(eventAttendeeRoute.Id, cancellationToken);
        }

        public async Task DeleteEventAttendeeRouteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var requestUri = string.Concat(Controller, $"/{id}");

            using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }
}