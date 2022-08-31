namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class UserRestService : RestServiceBase, IUserRestService
    {
        private readonly string UserApi = TrashMobServiceUrlBase + "users";

        public async Task<User> GetUserAsync(string userId)
        {
            try
            {
                //var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;

                //httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = new Uri(UserApi + "/" + userId);

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

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                //var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;

                //httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = new Uri(UserApi);

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

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                //var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Put;

                //httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
                httpRequestMessage.RequestUri = new Uri(UserApi);

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
