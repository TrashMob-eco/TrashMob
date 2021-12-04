namespace TrashMobMobile.Services
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class MobEventRestService : RestServiceBase, IMobEventRestService
    {
        private readonly string EventsApi = TrashMobServiceUrlBase + "events";

        public async Task<IEnumerable<MobEvent>> GetEventsAsync()
        {
            var mobEvents = new List<MobEvent>();

            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(EventsApi);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    mobEvents = JsonConvert.DeserializeObject<List<MobEvent>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return mobEvents;
        }

        public async Task<MobEvent> GetEventAsync(Guid eventId)
        {
            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(EventsApi + "/" + eventId);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MobEvent>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<MobEvent> UpdateEventAsync(MobEvent mobEvent)
        {
            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Put;

                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = new Uri(EventsApi + "/" + mobEvent.Id);

                httpRequestMessage.Content = JsonContent.Create(mobEvent, typeof(MobEvent), null, SerializerOptions);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
                string responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<MobEvent>(responseString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<MobEvent> AddEventAsync(MobEvent mobEvent)
        {
            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;

                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = new Uri(EventsApi);

                httpRequestMessage.Content = JsonContent.Create(mobEvent, typeof(MobEvent), null, SerializerOptions);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
                string responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<MobEvent>(responseString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task DeleteEventAsync(CancelEvent cancelEvent)
        {
            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Delete;

                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = new Uri(EventsApi);

                httpRequestMessage.Content = JsonContent.Create(cancelEvent, typeof(CancelEvent), null, SerializerOptions);

                HttpClient client = new HttpClient();
                _ = await client.SendAsync(httpRequestMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<MobEvent>> GetEventsUserIsAttending(Guid userId)
        {
            var mobEvents = new List<MobEvent>();

            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(EventsApi + $"/eventsuserisattending/{userId}");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
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
