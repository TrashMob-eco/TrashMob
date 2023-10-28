﻿namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

    public class EventSummaryRestService : RestServiceBase, IEventSummaryRestService
    {
        protected override string Controller => "eventsummaries";

        public EventSummaryRestService(IOptions<Settings> settings)
            : base(settings)
        {
        }

        public async Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/" + eventId;

                using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonConvert.DeserializeObject<List<EventSummary>>(content);
                    return result.FirstOrDefault();
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

                using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
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

                using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
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
