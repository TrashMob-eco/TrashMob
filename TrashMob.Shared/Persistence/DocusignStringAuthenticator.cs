namespace TrashMob.Shared.Persistence
{
    using DocuSign.eSign.Client;
    using System.Collections.Generic;
    using System.Text;
    using static DocuSign.eSign.Client.Auth.OAuth;

    public class DocusignStringAuthenticator : IDocusignAuthenticator
    {
        private readonly IKeyVaultManager keyVaultManager;

        public DocusignStringAuthenticator(IKeyVaultManager keyVaultManager)
        {
            this.keyVaultManager = keyVaultManager;
        }

        /// <summary>
        /// Uses Json Web Token (JWT) Authentication Method to obtain the necessary information needed to make API calls.
        /// </summary>
        /// <returns>Auth token needed for API calls</returns>
        public OAuthToken AuthenticateWithJWT(string clientId, string impersonatedUserId, string authServer)
        {
            var privateKey = keyVaultManager.GetSecret("DocusignPrivateKey");

            var apiClient = new ApiClient();
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };

            var bytes = Encoding.UTF8.GetBytes(privateKey);
            return apiClient.RequestJWTUserToken(clientId, impersonatedUserId, authServer, bytes, 1, scopes);
        }
    }
}
