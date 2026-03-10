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

        /// <summary>
        /// When true, routes requests through the v2 API (api/v2/{controller}).
        /// Override in derived services to opt in to v2 endpoints.
        /// </summary>
        protected virtual bool UseV2 => false;

        private string VersionPrefix => UseV2 ? "v2/" : "";

        protected HttpClient AuthorizedHttpClient
        {
            get
            {
                var authorizedHttpClient = httpClientFactory.CreateClient("ServerAPI");

                authorizedHttpClient.BaseAddress = new Uri(string.Concat(TrashMobApiAddress, VersionPrefix, Controller));

                return authorizedHttpClient;
            }
        }

        protected HttpClient CreateHttpClient(string basePath)
        {
            var client = httpClientFactory.CreateClient("ServerAPI");
            client.BaseAddress = new Uri(string.Concat(TrashMobApiAddress, VersionPrefix, basePath));
            return client;
        }

        protected HttpClient AnonymousHttpClient
        {
            get
            {
                var anonymousHttpClient = httpClientFactory.CreateClient("ServerAPI.Anonymous");
                anonymousHttpClient.BaseAddress = new Uri(string.Concat(TrashMobApiAddress, VersionPrefix, Controller));
                anonymousHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");
                return anonymousHttpClient;
            }
        }
    }
}