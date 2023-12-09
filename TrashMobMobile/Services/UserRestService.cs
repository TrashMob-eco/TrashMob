namespace TrashMobMobile.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Authentication;
    using TrashMobMobile.Config;

    public class UserRestService : RestServiceBase, IUserRestService
    {
        protected override string Controller => "users";

        public UserRestService(IOptions<Settings> settings) 
            : base(settings)
        {
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/" + userId;

                using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
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
                    BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller))
                };

                localHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(userContext);
                localHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");

                var requestUri = Controller + "/getuserbyemail/" + email;

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

                using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
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

                using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
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
