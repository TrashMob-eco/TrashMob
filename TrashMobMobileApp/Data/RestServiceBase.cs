namespace TrashMobMobileApp.Data
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;

    public class RestServiceBase
    {
        protected HttpClient HttpClient { get; private set; }

        protected JsonSerializerOptions SerializerOptions { get; private set; }

        private readonly IB2CAuthenticationService b2CAuthenticationService;

        protected RestServiceBase(HttpClient httpClient, IB2CAuthenticationService b2CAuthenticationService)
        {
            HttpClient = httpClient;
            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            this.b2CAuthenticationService = b2CAuthenticationService;
        }

        protected async Task<UserContext> GetUserContext()
        {
            return await (b2CAuthenticationService as B2CAuthenticationService).SignInAsync();
        }

        protected HttpRequestMessage GetDefaultHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("Accept", "application/json, text/plain");
            return httpRequestMessage;
        }
    }
}