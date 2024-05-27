namespace TrashMobMobile.Data
{
    using System.Text.Json;
    using TrashMobMobile.Authentication;
    using TrashMobMobile.Config;

    public abstract class RestServiceBase
    {
        private HttpClient authorizedHttpClient;

        protected RestServiceBase()
        {
            TrashMobApiAddress = Settings.ApiBaseUrl;

            authorizedHttpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller)),
            };

            authorizedHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
            authorizedHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(UserState.UserContext);

            AnonymousHttpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller)),
            };

            AnonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        protected static JsonSerializerOptions SerializerOptions { get; } = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        protected string TrashMobApiAddress { get; }

        protected abstract string Controller { get; }

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

                    authorizedHttpClient = new HttpClient
                    {
                        BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller)),
                    };

                    authorizedHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
                    authorizedHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(UserState.UserContext);
                }

                return authorizedHttpClient;
            }
        }

        protected HttpClient AnonymousHttpClient { get; }

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