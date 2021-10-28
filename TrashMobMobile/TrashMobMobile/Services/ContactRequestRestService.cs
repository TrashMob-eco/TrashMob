namespace TrashMobMobile.Services
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class ContactRequestRestService : RestServiceBase
    {
        private readonly Uri ContactRequestApi = new Uri(TrashMobServiceUrlBase + "contactrequests");

        public async Task AddContactRequest(ContactRequest contactRequest)
        {
            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;             
                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = ContactRequestApi;

                httpRequestMessage.Content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
    }
}
