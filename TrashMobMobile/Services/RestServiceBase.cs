namespace TrashMobMobile.Services
{
    using System.Text.Json;
    using TrashMobMobile.Authentication;
    using TrashMobMobile.Config;

    public abstract class RestServiceBase(IHttpClientFactory httpClientFactory)
    {
        protected static JsonSerializerOptions SerializerOptions { get; } = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        protected string TrashMobApiAddress => Settings.ApiBaseUrl;

        protected abstract string Controller { get; }

        protected HttpClient AuthorizedHttpClient
        {
            get
            {
                var authorizedHttpClient = httpClientFactory.CreateClient("ServerAPI");

                authorizedHttpClient.BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller));
                authorizedHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
                authorizedHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(UserState.UserContext);

                return authorizedHttpClient;
            }
        }

        protected HttpClient AnonymousHttpClient
        {
            get
            {
                var anonymousHttpClient = httpClientFactory.CreateClient("ServerAPI.Anonymous");
                anonymousHttpClient.BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller));
                anonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
                return anonymousHttpClient;
            }
        }

        protected virtual System.Net.Http.Headers.AuthenticationHeaderValue? GetAuthToken(UserContext userContext)
        {
            if (string.IsNullOrWhiteSpace(userContext.AccessToken))
            {
                return null;
            }

            return new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", userContext.AccessToken);
        }
    }
}