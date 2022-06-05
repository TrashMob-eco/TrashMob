namespace TrashMob.Shared.Persistence
{
    using DocuSign.eSign.Client;
    using System;
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
            var privateKeyEncoded = keyVaultManager.GetSecret("DocusignPrivateKeyEncoded");
            var privateKey = Convert.FromBase64String(privateKeyEncoded);

            var apiClient = new ApiClient();
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };

            return apiClient.RequestJWTUserToken(clientId, impersonatedUserId, authServer, privateKey, 1, scopes);
        }
    }
}
