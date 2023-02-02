namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

    public class EventAttendeeRestService : RestServiceBase, IEventAttendeeRestService
    {
        private const string EventAttendeesApi = "eventattendees";
        private readonly HttpClient httpClient;

        public EventAttendeeRestService(IOptions<Settings> settings)
            : base(settings)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, EventAttendeesApi))
            };

            httpClient.DefaultRequestHeaders.Authorization = GetAuthToken();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);
                var response = await httpClient.PostAsync(EventAttendeesApi, content, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = string.Concat(EventAttendeesApi, $"/{eventAttendee.EventId}/{eventAttendee.UserId}");

                using (var response = await httpClient.DeleteAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }

            return;
        }
    }
}
