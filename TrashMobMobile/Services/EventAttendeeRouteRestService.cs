namespace TrashMobMobile.Data
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Config;

    public class EventAttendeeRouteRestService : RestServiceBase, IEventAttendeeRouteRestService
    {
        protected override string Controller => "eventattendeeroutes";

        public EventAttendeeRouteRestService(IOptions<Settings> settings)
            : base(settings)
        {
        }

        public async Task AddAttendeeRouteAsync(EventAttendeeRoute eventAttendeeRoute, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventAttendeeRoute, typeof(EventAttendeeRoute), null, SerializerOptions);
                var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task RemoveAttendeeRouteAsync(EventAttendeeRoute eventAttendeeRoute, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = string.Concat(Controller, $"/{eventAttendeeRoute.EventId}/{eventAttendeeRoute.UserId}/{eventAttendeeRoute.Id}");

                using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
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
