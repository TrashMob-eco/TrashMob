namespace FlashTrashMob.Web.Persistence
{
    using FlashTrashMob.Web.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class MobDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MobDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<CleanupEvent> CleanupEvents { get; set; }

        public virtual DbSet<Rsvp> Rsvp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(configuration["DBEndpointUri"], configuration["DBPrimaryKey"]);
        }
    }
}
