namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;

    public class LocalKeyVaultManager : IKeyVaultManager
    {
        private readonly IConfiguration configuration;

        public LocalKeyVaultManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetSecret(string secretName)
        {
            return configuration[secretName];
        }
    }
}
