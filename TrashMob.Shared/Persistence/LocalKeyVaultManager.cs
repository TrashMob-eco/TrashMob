namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence.Interfaces;

    public class LocalKeyVaultManager : IKeyVaultManager
    {
        private readonly IConfiguration configuration;

        public LocalKeyVaultManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<X509Certificate2> GetCertificateAsync(string certificateName)
        {
            throw new System.NotImplementedException();
        }

        public string GetSecret(string secretName)
        {
            return configuration[secretName];
        }
    }
}
