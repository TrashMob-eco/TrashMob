namespace TrashMob.Persistence
{
    using TrashMob.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class MobDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MobDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<MobEvent> MobEvents { get; set; }

        public virtual DbSet<Rsvp> Rsvp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration["TMDBServerConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MobEvent>().ToTable("MobEvents");
            modelBuilder.Entity<Rsvp>().ToTable("Rsvps");
        }
    }
}
