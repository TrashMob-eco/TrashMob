namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

    public class EventPartnerLocationServiceRestService : RestServiceBase, IEventPartnerLocationServiceRestService
    {
        protected override string Controller => "eventpartnerlocationservices";

        public EventPartnerLocationServiceRestService(IOptions<Settings> settings)
            : base(settings)
        {
        }

        public async Task<PartnerLocation> GetHaulingPartnerLocationAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/gethaulingpartnerlocation/" + eventId;

                using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = JsonConvert.DeserializeObject<List<PartnerLocation>>(content);
                    return result.FirstOrDefault();
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
