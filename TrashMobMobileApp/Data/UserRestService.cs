namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Config;

    public class UserRestService : RestServiceBase, IUserRestService
    {
        private readonly string UserApi = "users";
        private readonly HttpClient httpClient;

        public UserRestService(IOptions<Settings> settings) 
            : base(settings)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, UserApi))
            };

            httpClient.DefaultRequestHeaders.Authorization = GetAuthToken();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = UserApi + "/" + userId;

                using (var response = await httpClient.GetAsync(requestUri, cancellationToken))
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

        public async Task<User> GetUserByEmailAsync(string email, UserContext userContext, CancellationToken cancellationToken = default)
        {
            try
            {
                var localHttpClient = new HttpClient
                {
                    BaseAddress = new Uri(string.Concat(TrashMobApiAddress, UserApi))
                };

                localHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(userContext);
                localHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");

                var requestUri = UserApi + "/getuserbyemail/" + email;

                using (var response = await localHttpClient.GetAsync(requestUri, cancellationToken))
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

                using (var response = await httpClient.PostAsync(UserApi, content, cancellationToken))
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

                using (var response = await httpClient.PutAsync(UserApi, content, cancellationToken))
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
