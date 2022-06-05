namespace TrashMob.Shared.Persistence
{
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Models;
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient secretClient;
        private readonly IKeyVaultClient keyVaultClient;

        public KeyVaultManager(SecretClient secretClient, IKeyVaultClient keyVaultClient)
        {
            this.secretClient = secretClient;
            this.keyVaultClient = keyVaultClient;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string certificateSecretName)
        {
            CertificateBundle cert = await keyVaultClient.GetCertificateAsync(secretClient.VaultUri.ToString(), certificateSecretName).ConfigureAwait(false);

            if (cert == null)
            {
                throw new InvalidOperationException($"Unable to find certificate with secret name {certificateSecretName}");
            }

            var secret = await keyVaultClient.GetSecretAsync(cert.SecretIdentifier.Identifier).ConfigureAwait(false);
            var pfxBytes = Convert.FromBase64String(secret.Value);
            return new X509Certificate2(pfxBytes, string.Empty, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }

        public string GetSecret(string secretName)
        {
            var keyValueSecret = secretClient.GetSecret(secretName);

            return keyValueSecret.Value.Value;
        }
    }
}
