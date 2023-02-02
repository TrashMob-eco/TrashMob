namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Config;

    public class MapRestService : RestServiceBase, IMapRestService
    {
        private readonly string MapsApi = "maps";
        private readonly HttpClient httpClient;

        public MapRestService(IOptions<Settings> settings)
            : base(settings)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, MapsApi))
            };

            httpClient.DefaultRequestHeaders.Authorization = GetAuthToken();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        public async Task<Address> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = MapsApi + $"/GetAddress?latitude={latitude}&longitude={longitude}";

                using (var response = await httpClient.GetAsync(requestUri, cancellationToken))
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
