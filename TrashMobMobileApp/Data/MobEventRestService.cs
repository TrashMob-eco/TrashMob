namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class MobEventRestService : RestServiceBase, IMobEventRestService
    {
        private readonly string EventsApi = "events";

        public MobEventRestService(HttpClient httpClient, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClient, b2CAuthenticationService)
        {
        }

        public async Task<IEnumerable<MobEvent>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<MobEvent>();

            try
            {
                var requestUri = $"{EventsApi}/active";

                using (var response = await HttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    mobEvents = JsonConvert.DeserializeObject<List<MobEvent>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }

            return mobEvents;
        }

        public async Task<IEnumerable<MobEvent>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<MobEvent>();

            try
            {
                var requestUri = $"{EventsApi}/userevents/{userId}/{showFutureEventsOnly}";

                using (var response = await HttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    mobEvents = JsonConvert.DeserializeObject<List<MobEvent>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return mobEvents;
        }

        public async Task<MobEvent> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = EventsApi + "/" + eventId;

                using (var response = await HttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return JsonConvert.DeserializeObject<MobEvent>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<MobEvent> UpdateEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = EventsApi + "/" + mobEvent.Id;

                var content = JsonContent.Create(mobEvent, typeof(MobEvent), null, SerializerOptions);

                using (var response = await HttpClient.PutAsync(requestUri, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    return JsonConvert.DeserializeObject<MobEvent>(returnContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<MobEvent> AddEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(mobEvent, typeof(MobEvent), null, SerializerOptions);

                using (var response = await HttpClient.PostAsync(EventsApi, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string returnContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MobEvent>(returnContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task DeleteEventAsync(CancelEvent cancelEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(cancelEvent, typeof(CancelEvent), null, SerializerOptions);

                // TODO: fix this
                using (var response = await HttpClient.DeleteAsync(EventsApi, cancellationToken))
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

        public async Task<IEnumerable<MobEvent>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default)
        {
            var mobEvents = new List<MobEvent>();

            try
            {
                var requestUri = EventsApi + $"/eventsuserisattending/{userId}";

                using (var response = await HttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    mobEvents = JsonConvert.DeserializeObject<List<MobEvent>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return mobEvents;
        }
    }
}
