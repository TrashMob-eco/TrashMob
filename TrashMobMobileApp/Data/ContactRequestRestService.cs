namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

    public class ContactRequestRestService : RestServiceBase, IContactRequestRestService
    {
        private readonly string ContactRequestApiPath = "contactrequest";
        private readonly HttpClient httpClient;

        public ContactRequestRestService(IOptions<Settings> settings) 
            : base(settings)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, ContactRequestApiPath))
            };

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        public async Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                contactRequest.Id = Guid.NewGuid();
                var content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

                using (var response = await httpClient.PostAsync(ContactRequestApiPath, content, cancellationToken))
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
