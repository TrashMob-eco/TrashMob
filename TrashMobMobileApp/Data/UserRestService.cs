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

        public UserRestService(HttpClient httpClient, IB2CAuthenticationService b2CAuthenticationService)
            : base(httpClient, b2CAuthenticationService)
        {
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = UserApi + "/" + userId;

                using (var response = await HttpClient.GetAsync(requestUri, cancellationToken))
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

                using (var response = await HttpClient.PostAsync(UserApi, content, cancellationToken))
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

                using (var response = await HttpClient.PutAsync(UserApi, content, cancellationToken))
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
