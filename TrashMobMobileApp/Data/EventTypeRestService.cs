namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;

    public class EventTypeRestService : RestServiceBase, IEventTypeRestService
    {
        private readonly string EventTypesApi = "eventtypes";

        public EventTypeRestService(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClientService, b2CAuthenticationService)
        {
        }

        public async Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var anonymousHttpClient = HttpClientService.CreateAnonymousClient();

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
