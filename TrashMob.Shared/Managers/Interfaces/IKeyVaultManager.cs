namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines operations for retrieving secrets and certificates from Azure Key Vault.
    /// </summary>
    public interface IKeyVaultManager
    {
        /// <summary>
        /// Gets a secret value from Key Vault.
        /// </summary>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>The secret value.</returns>
        public string GetSecret(string secretName);

        /// <summary>
        /// Gets a certificate from Key Vault.
        /// </summary>
        /// <param name="certificateName">The name of the certificate.</param>
        /// <returns>The X509 certificate.</returns>
        public Task<X509Certificate2> GetCertificateAsync(string certificateName);
    }
}