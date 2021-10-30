namespace TrashMobMobile.Services
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class UserRestService : RestServiceBase, IUserRestService
    {
        private readonly Uri UserApi = new Uri(TrashMobServiceUrlBase + "users");

        public async Task<User> AddUser(User user)
        {
            try
            {
                var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;

                httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = UserApi;

                httpRequestMessage.Content = JsonContent.Create(user, typeof(User), null, SerializerOptions);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
                string responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<User>(responseString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }
    }
}
