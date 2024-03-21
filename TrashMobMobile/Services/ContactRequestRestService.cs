namespace TrashMobMobile.Data;

using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrashMob.Models;

public class ContactRequestRestService : RestServiceBase, IContactRequestRestService
{
    protected override string Controller => "contactrequest";

    public ContactRequestRestService()
    {
    }

    public async Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            contactRequest.Id = Guid.NewGuid();
            var content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

            using (var response = await AnonymousHttpClient.PostAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }
}
