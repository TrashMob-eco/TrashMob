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

    public class EventSummaryRestService : RestServiceBase, IEventSummaryRestService
    {
        private readonly string EventSummaryApi = "eventsummaries";

        public EventSummaryRestService(HttpClient httpClient, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClient, b2CAuthenticationService)
        {
        }

        public async Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = EventSummaryApi + "/" + eventId;

                using (var response = await HttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return JsonConvert.DeserializeObject<EventSummary>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventSummary, typeof(EventSummary), null, SerializerOptions);

                using (var response = await HttpClient.PutAsync(EventSummaryApi, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }

                return await GetEventSummaryAsync(eventSummary.EventId, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventSummary, typeof(EventSummary), null, SerializerOptions);

                using (var response = await HttpClient.PostAsync(EventSummaryApi, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }

                return await GetEventSummaryAsync(eventSummary.EventId, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }
    }
}
