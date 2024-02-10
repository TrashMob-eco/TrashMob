namespace TrashMobMobile.Data
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Config;

    public class EventAttendeeRestService : RestServiceBase, IEventAttendeeRestService
    {
        protected override string Controller => "eventattendees";

        public EventAttendeeRestService(IOptions<Settings> settings)
            : base(settings)
        {
        }

        public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);
                var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
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
                var requestUri = string.Concat(Controller, $"/{eventAttendee.EventId}/{eventAttendee.UserId}");

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
