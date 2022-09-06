namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class UserRestService : RestServiceBase, IUserRestService
    {
        private readonly string UserApi = "users";

        public UserRestService(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClientService, b2CAuthenticationService)
        {
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = UserApi + "/" + userId;
                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();

                using (var response = await authorizedHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<User>(responseString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(user, typeof(User), null, SerializerOptions);

                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();

                using (var response = await authorizedHttpClient.PostAsync(UserApi, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<User>(responseString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(user, typeof(User), null, SerializerOptions);

                var authorizedHttpClient = HttpClientService.CreateAuthorizedClient();

                using (var response = await authorizedHttpClient.PutAsync(UserApi, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<User>(responseString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }
    }
}
