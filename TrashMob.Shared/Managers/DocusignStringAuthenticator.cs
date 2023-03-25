namespace TrashMob.Shared.Managers
{
    using DocuSign.eSign.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TrashMob.Shared.Managers.Interfaces;
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
        public OAuthToken AuthenticateWithJWT(string clientId, string impersonatedUserId, string authServer, out string baseUri)
        {
            baseUri = string.Empty;

            var privateKeyEncoded = keyVaultManager.GetSecret("DocusignPrivateKeyEncoded");
            var privateKey = Convert.FromBase64String(privateKeyEncoded);

            var apiClient = new DocuSignClient();
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };

            var token = apiClient.RequestJWTUserToken(clientId, impersonatedUserId, authServer, privateKey, 1, scopes);

            var userInfo = apiClient.GetUserInfo(token.access_token);

            if (userInfo?.Accounts?.Count > 0)
            {
                var account = userInfo.Accounts.FirstOrDefault(a => a.IsDefault == "true");

                if (account != null)
                {
                    baseUri = account.BaseUri + "/restapi";
                }
            }

            return token;
        }
    }
}
