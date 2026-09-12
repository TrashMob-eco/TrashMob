namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;

public class MapRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IMapRestService
{
    protected override string Controller => "maps";

    public async Task<Address> GetAddressAsync(double latitude, double longitude,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller +
            $"/address?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<AddressDto>(responseString)!.ToEntity();
        }
    }
}