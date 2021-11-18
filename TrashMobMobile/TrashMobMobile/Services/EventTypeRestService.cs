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

    public class EventTypeRestService : RestServiceBase, IEventTypeRestService
    {
        private readonly string EventTypesApi = TrashMobServiceUrlBase + "eventtypes";

        public async Task<IEnumerable<EventType>> GetEventTypesAsync()
        {
            var eventTypes = new List<EventType>();

            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(EventTypesApi);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    eventTypes = JsonConvert.DeserializeObject<List<EventType>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return eventTypes;
        }
    }
}
