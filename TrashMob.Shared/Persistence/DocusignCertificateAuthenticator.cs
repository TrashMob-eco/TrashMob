namespace TrashMob.Shared.Persistence
{
    using DocuSign.eSign.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using static DocuSign.eSign.Client.Auth.OAuth;

    public class DocusignCertificateAuthenticator : IDocusignAuthenticator
    {
        private readonly IKeyVaultManager keyVaultManager;

        public DocusignCertificateAuthenticator(IKeyVaultManager keyVaultManager)
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

            // TODO: is there a better way to do this
            // This has to be forced to be synchronous because a Span<byte> cannot be created in an Asynchronous method.
            var certificate = keyVaultManager.GetCertificateAsync("Docusign").GetAwaiter().GetResult();
            var pk = certificate.GetRSAPrivateKey();

            var bytes = new Span<byte>();
            pk.TryExportRSAPrivateKey(bytes, out var bytesWritten);

            var apiClient = new ApiClient();
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };

            var token = apiClient.RequestJWTUserToken(clientId, impersonatedUserId, authServer, bytes.ToArray(), 1, scopes);
            
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
