namespace TrashMobMobileApp.Data
{
    using System;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;

    public class ContactRequestRestService : RestServiceBase, IContactRequestRestService
    {
        private readonly string ContactRequestApiPath = "contactrequest";

        public ContactRequestRestService(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService) 
            : base(httpClientService, b2CAuthenticationService)
        {
        }

        public async Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                contactRequest.Id = Guid.NewGuid();
                var content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

                var anonymousHttpClient = HttpClientService.CreateAnonymousClient();

                using (var response = await anonymousHttpClient.PostAsync(ContactRequestApiPath, content, cancellationToken))
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
