namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides data access implementation for retrieving application secrets and configuration values.
    /// </summary>
    public class SecretRepository(IConfiguration configuration) : ISecretRepository
    {

        /// <inheritdoc />
        public string Get(string name)
        {
            return configuration[name];
        }
    }
}