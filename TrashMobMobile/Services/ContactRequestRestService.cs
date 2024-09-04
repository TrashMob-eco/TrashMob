namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Net.Http.Json;
using TrashMob.Models;

public class ContactRequestRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IContactRequestRestService
{
    protected override string Controller => "contactrequest";

    public async Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default)
    {
        contactRequest.Id = Guid.NewGuid();
        var content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

        using (var response = await AnonymousHttpClient.PostAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }
}