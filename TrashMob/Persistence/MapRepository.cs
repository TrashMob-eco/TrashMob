namespace TrashMob.Persistence
{
    using Microsoft.Extensions.Configuration;

    public class MapRepository : IMapRepository
    {
        private readonly IConfiguration configuration;
        private const string AzureMapKeyName = "AzureMapsDev";

        public MapRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetMapKey()
        {
            return configuration[AzureMapKeyName];
        }
    }
}
