namespace TrashMobMobile.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Config;

    public class EventTypeRestService : RestServiceBase, IEventTypeRestService
    {
        protected override string Controller => "eventtypes";

        public EventTypeRestService(IOptions<Settings> settings)
            : base(settings)
        {
        }

        public async Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
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
