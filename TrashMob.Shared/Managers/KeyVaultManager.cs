namespace TrashMob.Shared.Managers
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Azure.Security.KeyVault.Secrets;
    using TrashMob.Shared.Managers.Interfaces;

    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient secretClient;

        public KeyVaultManager(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string certificateSecretName)
        {
            var secret = await secretClient.GetSecretAsync(certificateSecretName).ConfigureAwait(false) 
                ?? throw new InvalidOperationException($"Unable to find certificate with secret name {certificateSecretName}");

            var pfxBytes = Convert.FromBase64String(secret.Value.Value);
            return X509CertificateLoader.LoadCertificate(pfxBytes);
        }

        public string GetSecret(string secretName)
        {
            var keyValueSecret = secretClient.GetSecret(secretName);

            return keyValueSecret.Value.Value;
        }
    }
}