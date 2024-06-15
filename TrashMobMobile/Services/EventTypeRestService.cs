namespace TrashMobMobile.Services;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models;

public class EventTypeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventTypeRestService
{
    protected override string Controller => "eventtypes";

    public async Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

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