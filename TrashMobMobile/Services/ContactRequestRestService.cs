namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Net.Http.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;

public class ContactRequestRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IContactRequestRestService
{
    protected override string Controller => "contactrequest";

    public async Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default)
    {
        var dto = contactRequest.ToV2Dto();
        var content = JsonContent.Create(dto, typeof(ContactRequestDto), null, SerializerOptions);

        using (var response = await AnonymousHttpClient.PostAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }
}