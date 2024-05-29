namespace TrashMobMobile.Data
{
    using System.Diagnostics;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models;

    public class EventSummaryRestService : RestServiceBase, IEventSummaryRestService
    {
        protected override string Controller => "eventsummaries";

        public async Task<EventSummary> GetEventSummaryAsync(Guid eventId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/" + eventId;

                using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        return JsonConvert.DeserializeObject<EventSummary>(content);
                    }
                    catch (HttpRequestException)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            var eventSummary = new EventSummary
                            {
                                EventId = eventId,
                                ActualNumberOfAttendees = 0,
                                DurationInMinutes = 0,
                                NumberOfBags = 0,
                                Notes = string.Empty,
                            };

                            return eventSummary;
                        }

                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary,
            CancellationToken cancellationToken = default)
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

        public async Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary,
            CancellationToken cancellationToken = default)
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