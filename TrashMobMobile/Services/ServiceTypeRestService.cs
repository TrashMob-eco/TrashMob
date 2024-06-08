namespace TrashMobMobile.Data;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMobMobile.Services;

public class ServiceTypeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IServiceTypeRestService
{
    protected override string Controller => "servicetypes";

    public async Task<IEnumerable<ServiceType>> GetServiceTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<List<ServiceType>>(responseString);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}