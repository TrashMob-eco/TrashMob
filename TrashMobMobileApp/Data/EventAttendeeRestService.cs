namespace TrashMobMobileApp.Data
{
    using System;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;

    public class EventAttendeeRestService : RestServiceBase, IEventAttendeeRestService
    {
        private readonly string EventAttendeeApi = "eventattendee";

        public EventAttendeeRestService(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClientService, b2CAuthenticationService)
        {
        }

        public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();
                var response = await authorizedHttpClient.PostAsync(EventAttendeeApi, content, cancellationToken);
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
                var requestUri = new Uri(EventAttendeeApi + $"/{eventAttendee.EventId}/{eventAttendee.UserId}");

                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();

                using (var response = await authorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
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
