namespace TrashMobMobile.Data
{
    using System.Text.Json;
    using TrashMobMobile.Authentication;
    using TrashMobMobile.Config;

    public abstract class RestServiceBase
    {
        protected static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected string TrashMobApiAddress { get; }

        protected abstract string Controller { get; }

        private HttpClient authorizedHttpClient;

        protected HttpClient AuthorizedHttpClient
        {
            get
            {
                if (authorizedHttpClient?.DefaultRequestHeaders.Authorization == null)
                {
                    if (authorizedHttpClient != null)
                    {
                        authorizedHttpClient.Dispose();
                    }

                    authorizedHttpClient = new HttpClient()
                    {
                        BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller))
                    };

                    authorizedHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
                    authorizedHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(UserState.UserContext);
                }

                return authorizedHttpClient;
            }
        }
        protected HttpClient AnonymousHttpClient { get; }

        protected RestServiceBase()
        {
            TrashMobApiAddress = Settings.ApiBaseUrl;

            authorizedHttpClient = new HttpClient()
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller))
            };

            authorizedHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
            authorizedHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(UserState.UserContext);

            AnonymousHttpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller))
            };

            AnonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        protected virtual System.Net.Http.Headers.AuthenticationHeaderValue GetAuthToken(UserContext userContext)
        {
            if (userContext == null || string.IsNullOrWhiteSpace(userContext.AccessToken))
            {
                return null;
            }

            return new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", userContext.AccessToken);
        }
    }
}