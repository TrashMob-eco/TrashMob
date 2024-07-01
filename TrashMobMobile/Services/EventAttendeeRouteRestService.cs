namespace TrashMobMobile.Services
{
    using System.Diagnostics;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models;

    public class EventAttendeeRouteRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventAttendeeRouteRestService
    {
        protected override string Controller => "eventattendeeroutes";

        public async Task<IEnumerable<EventAttendeeRoute>> GetEventAttendeeRoutesAsync(Guid eventId, 
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/" + eventId + "/" + userId;

                using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
                {
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        return JsonConvert.DeserializeObject<IEnumerable<EventAttendeeRoute>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<EventAttendeeRoute>> GetEventAttendeeRoutesForEventAsync(Guid eventId,
                CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/byeventid/" + eventId;

                using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return JsonConvert.DeserializeObject<IEnumerable<EventAttendeeRoute>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<EventAttendeeRoute>> GetEventAttendeeRoutesForUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/byuserid/" + userId;

                using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return JsonConvert.DeserializeObject<IEnumerable<EventAttendeeRoute>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventAttendeeRoute> GetEventAttendeeRouteAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/" + id;

                using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return JsonConvert.DeserializeObject<EventAttendeeRoute>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventAttendeeRoute> AddEventAttendeeRouteAsync(EventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventAttendeeRoute, typeof(EventAttendeeRoute), null, SerializerOptions);

                using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }

                return await GetEventAttendeeRouteAsync(eventAttendeeRoute.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task DeleteEventAttendeeRouteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = string.Concat(Controller, $"/{id}");

                using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }
    }
}