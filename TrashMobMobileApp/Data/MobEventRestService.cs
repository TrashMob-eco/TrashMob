namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Reflection.Metadata.Ecma335;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Models;

    public class MobEventRestService : RestServiceBase, IMobEventRestService
    {
        private readonly string EventsApi = "events";

        public MobEventRestService(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClientService, b2CAuthenticationService)
        {
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<Event>();

            try
            {
                var requestUri = $"{EventsApi}/active";
                var anonymousHttpClient = HttpClientService.CreateAnonymousClient();
                var response = await anonymousHttpClient.GetAsync(requestUri, cancellationToken);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                mobEvents = JsonConvert.DeserializeObject<List<Event>>(content);
            }
            catch (Exception ex) when (!ex.IsClosedStreamException())
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }

            return mobEvents;
        }

        public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<Event>();

            try
            {
                var requestUri = $"{EventsApi}/userevents/{userId}/{showFutureEventsOnly}";
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                var response = await authorizedHttpClient.GetAsync(requestUri, cancellationToken);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                mobEvents = JsonConvert.DeserializeObject<List<Event>>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return mobEvents;
        }

        public async Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = EventsApi + "/" + eventId;
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                var response = await authorizedHttpClient.GetAsync(requestUri, cancellationToken);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<Event>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(mobEvent, typeof(Event), null, SerializerOptions);
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                var response = await authorizedHttpClient.PutAsync(EventsApi, content, cancellationToken);
                response.EnsureSuccessStatusCode();
                string returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<Event>(returnContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = null;

            try
            {
                var content = JsonContent.Create(mobEvent, typeof(Event), null, SerializerOptions);
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                response = await authorizedHttpClient.PostAsync(EventsApi, content, cancellationToken);
                response.EnsureSuccessStatusCode();
                string returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<Event>(returnContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task DeleteEventAsync(EventCancellationRequest cancelEvent, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = null;

            try
            {
                var content = JsonContent.Create(cancelEvent, typeof(EventCancellationRequest), null, SerializerOptions);
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                var requestUri = new Uri(string.Concat(authorizedHttpClient.BaseAddress, EventsApi));
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    Content = content,
                    RequestUri = requestUri,
                };

                response = await authorizedHttpClient.SendAsync(httpRequestMessage, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<Event>();

            try
            {
                var requestUri = EventsApi + $"/eventsuserisattending/{userId}";
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                var response = await authorizedHttpClient.GetAsync(requestUri, cancellationToken);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                mobEvents = JsonConvert.DeserializeObject<List<Event>>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }

            return mobEvents;
        }
    }
}
