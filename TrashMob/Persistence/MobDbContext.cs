namespace TrashMob.Persistence
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Models;

    public class MobDbContext : IdentityDbContext
    {
        private readonly IConfiguration configuration;

        public MobDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<EventAttendee> EventAttendees { get; set; }

        public virtual DbSet<EventHistory> EventHistories { get; set; }

        public virtual DbSet<EventStatus> EventStatuses { get; set; }

        public virtual DbSet<EventType> EventTypes { get; set; }

        public virtual DbSet<NotificationType> NotificationTypes { get; set; }

        public virtual DbSet<UserFeedback> UserFeedback { get; set; }

        public virtual DbSet<UserFeedback> UserSubscription { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration["TMDBServerConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.PrivacyPolicyVersion).HasMaxLength(50);

                entity.Property(e => e.TermsOfServiceVersion).HasMaxLength(50);

                entity.HasOne(d => d.RecruitedByUser)
                    .WithMany(p => p.UsersRecruited)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ApplicationUser_RecruitedBy");
            });

            modelBuilder.Entity<AttendeeNotification>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.AttendeeNotifications)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_AttendeeNotification_Event");

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.AttendeeNotifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AttendeeNotification_ApplicationUser");

                entity.HasOne(d => d.NotificationType)
                    .WithMany(p => p.AttendeeNotifications)
                    .HasForeignKey(d => d.NotificationTypeId)
                    .HasConstraintName("FK_AttendeeNotification_NotificationType");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.Gpscoords)
                    .HasMaxLength(50)
                    .HasColumnName("GPSCoords");

                entity.Property(e => e.LastUpdatedByUserId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.StateProvince)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.HasOne(d => d.EventStatus)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.EventStatusId)
                    .HasConstraintName("FK_Events_EventStatuses");

                entity.HasOne(d => d.EventType)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.EventTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Events_EventTypes");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Events_ApplicationUser_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Events_ApplicationUser_LastUpdatedBy");
            });

            modelBuilder.Entity<EventAttendee>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.EventId });

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendees_Events");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendees_ApplicationUser");
            });

            modelBuilder.Entity<EventHistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventHistory");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.Gpscoords)
                    .HasMaxLength(50)
                    .HasColumnName("GPSCoords");

                entity.Property(e => e.LastUpdatedByUserId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.StateProvince)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<EventStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EventType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserFeedback>(entity =>
            {
                entity.ToTable("UserFeedback");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Comments).HasMaxLength(2000);

                entity.Property(e => e.RegardingUserId)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.RegardingUser)
                    .WithMany(p => p.UserFeedbackRegardingUsers)
                    .HasForeignKey(d => d.RegardingUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeedback_ApplicationUserRegarding");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFeedbackUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeedback_ApplicationUser");
            });

            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.FollowingId });

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.FollowingId)
                    .IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersFollowing)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSubscriptions_ApplicationUserFollowed");

                entity.HasOne(d => d.FollowingUser)
                    .WithMany(p => p.UsersFollowed)
                    .HasForeignKey(d => d.FollowingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSubscriptions_ApplicationUsersFollowing");
            });
        }
    }
}
