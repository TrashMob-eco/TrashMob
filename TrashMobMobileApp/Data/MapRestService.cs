namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class MapRestService : RestServiceBase, IMapRestService
    {
        private readonly string MapsApi = "maps";

        public MapRestService(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClientService, b2CAuthenticationService)
        {
        }

        public async Task<Address> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = MapsApi + $"/GetAddress?latitude={latitude}&longitude={longitude}";

                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();

                using (var response = await authorizedHttpClient.GetAsync(requestUri, cancellationToken))
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
