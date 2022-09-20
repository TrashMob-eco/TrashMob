namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Persistence.Interfaces;

    public class SecretRepository : ISecretRepository
    {
        private readonly IConfiguration configuration;

        public SecretRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetSecret(string name)
        {
            return configuration[name];
        }
    }
}
