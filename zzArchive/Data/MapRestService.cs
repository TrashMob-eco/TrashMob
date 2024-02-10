namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

    public class MapRestService : RestServiceBase, IMapRestService
    {
        protected override string Controller => "maps";

        public MapRestService(IOptions<Settings> settings)
            : base(settings)
        {
        }

        public async Task<Address> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + $"/GetAddress?latitude={latitude}&longitude={longitude}";

                using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<Address>(responseString);
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
