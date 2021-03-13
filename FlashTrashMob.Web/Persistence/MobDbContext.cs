namespace FlashTrashMob.Web.Persistence
{
    using System.Data.Entity;
    using FlashTrashMob.Web.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class MobDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<CleanupEvent> CleanupEvents { get; set; }

        public virtual DbSet<Rsvp> Rsvp { get; set; }

        public MobDbContext()
        {
            Database.EnsureCreatedAsync().Wait();
        }
    }
}
