namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides data access implementation for retrieving application secrets and configuration values.
    /// </summary>
    public class SecretRepository : ISecretRepository
    {
        /// <summary>
        /// The configuration provider.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration provider to use for retrieving secrets.</param>
        public SecretRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public string Get(string name)
        {
            return configuration[name];
        }
    }
}