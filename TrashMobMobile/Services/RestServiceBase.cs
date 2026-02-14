namespace TrashMobMobile.Services
{
    using System.Text.Json;
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
    }
}