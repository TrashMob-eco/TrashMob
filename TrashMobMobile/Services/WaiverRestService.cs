namespace TrashMobMobile.Data;

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;

public class WaiverRestService : RestServiceBase, IWaiverRestService
{
    protected override string Controller => "waivers";

    public WaiverRestService()
    {
    }

    public async Task<Waiver> GetWaiver(string waiverName, CancellationToken cancellationToken)
    {
        try
        {
            var requestUri = Controller + "/" + waiverName;
            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<Waiver>(responseString);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}
