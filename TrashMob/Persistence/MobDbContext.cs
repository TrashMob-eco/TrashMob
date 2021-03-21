namespace TrashMob.Persistence
{
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Models;

    public class MobDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        private readonly IConfiguration configuration;

        public MobDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            IConfiguration configuration) : base(options, operationalStoreOptions)
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
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MobEvent>().ToTable("MobEvents");
            modelBuilder.Entity<Rsvp>().ToTable("Rsvps");
        }
    }
}
