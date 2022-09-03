namespace TrashMobMobileApp.Data
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class EventAttendeeRestService : RestServiceBase, IEventAttendeeRestService
    {
        private readonly string EventAttendeeApi = "eventattendee";

        public EventAttendeeRestService(HttpClient httpClient, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClient, b2CAuthenticationService)
        {
        }

        public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);

                using (var response = await HttpClient.PostAsync(EventAttendeeApi, content, cancellationToken))
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

        public async Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = new Uri(EventAttendeeApi + $"/{eventAttendee.EventId}/{eventAttendee.UserId}");

                using (var response = await HttpClient.DeleteAsync(requestUri, cancellationToken))
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
