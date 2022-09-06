namespace TrashMobMobileApp.Data
{
    public class HttpClientService
    {
        private const string AssemblyName = "TrashMobMobileApp";

        private IHttpClientFactory HttpClientFactory { get; }
        
        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public HttpClient CreateAnonymousClient()
        {
            return HttpClientFactory.CreateClient($"{AssemblyName}.ServerAPI.Anonymous");
        }

        public HttpClient CreateAuthorizedClient()
        {
            return HttpClientFactory.CreateClient($"{AssemblyName}.ServerAPI");
        }
    }
}
