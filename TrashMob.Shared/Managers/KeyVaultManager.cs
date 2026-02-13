namespace TrashMob.Shared.Managers
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Azure.Security.KeyVault.Secrets;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Azure Key Vault implementation for retrieving secrets and certificates.
    /// </summary>
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient secretClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultManager"/> class.
        /// </summary>
        /// <param name="secretClient">The Azure Key Vault secret client.</param>
        public KeyVaultManager(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        /// <inheritdoc />
        public async Task<X509Certificate2> GetCertificateAsync(string certificateSecretName)
        {
            var secret = await secretClient.GetSecretAsync(certificateSecretName)
                ?? throw new InvalidOperationException($"Unable to find certificate with secret name {certificateSecretName}");

            var pfxBytes = Convert.FromBase64String(secret.Value.Value);
            return X509CertificateLoader.LoadCertificate(pfxBytes);
        }

        /// <inheritdoc />
        public string GetSecret(string secretName)
        {
            var keyValueSecret = secretClient.GetSecret(secretName);

            return keyValueSecret.Value.Value;
        }
    }
}