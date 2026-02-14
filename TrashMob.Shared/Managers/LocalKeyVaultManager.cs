namespace TrashMob.Shared.Managers
{
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Local development implementation of key vault manager that retrieves secrets from configuration.
    /// </summary>
    public class LocalKeyVaultManager(IConfiguration configuration) : IKeyVaultManager
    {

        /// <inheritdoc />
        public Task<X509Certificate2> GetCertificateAsync(string certificateName)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string GetSecret(string secretName)
        {
            return configuration[secretName];
        }
    }
}