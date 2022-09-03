namespace TrashMobMobileApp.Data
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class ContactRequestRestService : RestServiceBase, IContactRequestRestService
    {
        private readonly string ContactRequestApiPath = "contactrequest";

        public ContactRequestRestService(HttpClient httpClient, IB2CAuthenticationService b2CAuthenticationService) 
            : base(httpClient, b2CAuthenticationService)
        {
        }

        public async Task AddContactRequest(ContactRequest contactRequest)
        {
            try
            {
                contactRequest.Id = Guid.NewGuid().ToString();
                var content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

                using (var response = await HttpClient.PostAsync(ContactRequestApiPath, content))
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
