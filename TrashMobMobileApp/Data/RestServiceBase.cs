namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using System.Text.Json;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Config;

    public class RestServiceBase
    {
        protected JsonSerializerOptions SerializerOptions { get; private set; }
        
        protected string TrashMobApiAddress { get; }

        protected RestServiceBase(IOptions<Settings> settings)
        {
            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            TrashMobApiAddress = settings.Value.ApiBaseUrl;
        }

        protected virtual System.Net.Http.Headers.AuthenticationHeaderValue GetAuthToken()
        {
            return new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", UserState.UserContext.AccessToken);
        }

        protected virtual System.Net.Http.Headers.AuthenticationHeaderValue GetAuthToken(UserContext userContext)
        {
            return new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", userContext.AccessToken);
        }
    }
}