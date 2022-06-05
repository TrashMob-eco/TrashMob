namespace TrashMob.Shared.Persistence
{
    using Azure.Security.KeyVault.Secrets;
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient secretClient;

        public KeyVaultManager(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string certificateSecretName)
        {
            var secret = await secretClient.GetSecretAsync(certificateSecretName).ConfigureAwait(false);
            
            if (secret == null)
            {
                throw new InvalidOperationException($"Unable to find certificate with secret name {certificateSecretName}");
            }

            var pfxBytes = Convert.FromBase64String(secret.Value.Value);
            return new X509Certificate2(pfxBytes, string.Empty, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
        }

        public string GetSecret(string secretName)
        {
            var keyValueSecret = secretClient.GetSecret(secretName);

            return keyValueSecret.Value.Value;
        }
    }
}
