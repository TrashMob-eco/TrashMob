namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMobMobile.Models;

public class DocusignRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IDocusignRestService
{
    protected override string Controller => "docusign";

    public async Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest,
        CancellationToken cancellationToken)
    {
        var content = JsonContent.Create(envelopeRequest, typeof(EnvelopeRequest), null, SerializerOptions);

        var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<EnvelopeResponse>(returnContent);
    }
}