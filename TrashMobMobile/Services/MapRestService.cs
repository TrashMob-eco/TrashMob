namespace TrashMobMobile.Data;

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TrashMob.Models;

public class MapRestService : RestServiceBase, IMapRestService
{
    protected override string Controller => "maps";

    public MapRestService()
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
