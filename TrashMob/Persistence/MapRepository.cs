namespace TrashMob.Persistence
{
    using Microsoft.Extensions.Configuration;

    public class MapRepository : IMapRepository
    {
        private readonly IConfiguration configuration;
        private const string GoogleMapKeyName = "GoogleMapKey";

        public MapRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetMapKey()
        {
            return configuration[GoogleMapKeyName];
        }
    }
}
