namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Models;

    public class MobEventRestService : RestServiceBase, IMobEventRestService
    {
        private readonly string EventsApi = "events";
        private readonly HttpClient httpClient;
        private readonly HttpClient anonymousHttpClient;

        public MobEventRestService(IOptions<Settings> settings)
            : base(settings)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, EventsApi))
            };

            httpClient.DefaultRequestHeaders.Authorization = GetAuthToken();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");

            anonymousHttpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, EventsApi))
            };

            anonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<Event>();

            try
            {
                var requestUri = $"{EventsApi}/active";
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
                var response = await httpClient.GetAsync(requestUri, cancellationToken);
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
                var response = await httpClient.GetAsync(requestUri, cancellationToken);
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
                var response = await httpClient.PutAsync(EventsApi, content, cancellationToken);
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
                response = await httpClient.PostAsync(EventsApi, content, cancellationToken);
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
                var requestUri = new Uri(string.Concat(httpClient.BaseAddress, EventsApi));
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    Content = content,
                    RequestUri = requestUri,
                };

                response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
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
                var response = await httpClient.GetAsync(requestUri, cancellationToken);
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
