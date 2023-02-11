namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using System.Text.Json;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Config;

    public abstract class RestServiceBase
    {
        protected JsonSerializerOptions SerializerOptions { get; private set; }
        
        protected string TrashMobApiAddress { get; }

        protected abstract string Controller { get; }

        protected HttpClient AuthorizedHttpClient { get; }

        protected HttpClient AnonymousHttpClient { get; }

        protected RestServiceBase(IOptions<Settings> settings)
        {
            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            TrashMobApiAddress = settings.Value.ApiBaseUrl;

            AuthorizedHttpClient = new HttpClient()
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller))
            };

            AuthorizedHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
            AuthorizedHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(UserState.UserContext);

            AnonymousHttpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller))
            };

            AnonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
        }

        protected virtual System.Net.Http.Headers.AuthenticationHeaderValue GetAuthToken(UserContext userContext)
        {
            return new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", userContext.AccessToken);
        }
    }
}