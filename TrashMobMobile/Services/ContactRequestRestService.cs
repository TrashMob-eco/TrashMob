namespace TrashMobMobile.Data
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Config;

    public class ContactRequestRestService : RestServiceBase, IContactRequestRestService
    {
        protected override string Controller => "contactrequest";

        public ContactRequestRestService(IOptions<Settings> settings) 
            : base(settings)
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
}
