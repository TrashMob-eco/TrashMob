namespace TrashMobMobile.Data;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models;

public class MapRestService : RestServiceBase, IMapRestService
{
    protected override string Controller => "maps";

    public async Task<Address> GetAddressAsync(double latitude, double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + $"/GetAddress?latitude={latitude}&longitude={longitude}";

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

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