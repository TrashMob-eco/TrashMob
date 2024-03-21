namespace TrashMobMobile.Data;

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TrashMobMobile.Models;

public class DocusignRestService : RestServiceBase, IDocusignRestService
{
    protected override string Controller => "docusign";

    public DocusignRestService()
    {
    }

    public async Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest, CancellationToken cancellationToken)
    {
        try
        {
            var content = JsonContent.Create(envelopeRequest, typeof(EnvelopeRequest), null, SerializerOptions);
            
            HttpResponseMessage response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            string returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<EnvelopeResponse>(returnContent);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}
