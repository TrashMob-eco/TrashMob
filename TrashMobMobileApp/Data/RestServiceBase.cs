namespace TrashMobMobileApp.Data
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
//    using TrashMobMobile.Features.LogOn;

    public class RestServiceBase
    {
        protected HttpClient Client { get; private set; }

        protected JsonSerializerOptions SerializerOptions { get; private set; }

        protected const string TrashMobServiceUrlBase = "https://as-tm-dev-westus2.azurewebsites.net/api/";

        protected RestServiceBase()
        {
            Client = new HttpClient();
            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        //protected async Task<UserContext> GetUserContext()
        //{
        //    return await B2CAuthenticationService.Instance.SignInAsync();
        //}

        protected HttpRequestMessage GetDefaultHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("Accept", "application/json, text/plain");
            return httpRequestMessage;
        }
    }
}