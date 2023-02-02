namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

    public class EventTypeRestService : RestServiceBase, IEventTypeRestService
    {
        private readonly string EventTypesApi = "eventtypes";
        private readonly HttpClient anonymousHttpClient;

        public EventTypeRestService(IOptions<Settings> settings)
            : base(settings)
        {
            anonymousHttpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, EventTypesApi))
            };

            anonymousHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken();
            anonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        public async Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var response = await anonymousHttpClient.GetAsync(EventTypesApi, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<List<EventType>>(responseString);
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
