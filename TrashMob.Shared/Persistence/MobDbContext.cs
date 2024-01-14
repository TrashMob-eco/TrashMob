namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;

    public class MobDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MobDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;            
        }

        public virtual DbSet<ContactRequest> ContactRequests { get; set; }

        public virtual DbSet<EventPartnerLocationService> EventPartnerLocationServices { get; set; }

        public virtual DbSet<EventAttendee> EventAttendees { get; set; }

        public virtual DbSet<EventAttendeeRoute> EventAttendeeRoutes { get; set; }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<EventPartnerLocationServiceStatus> EventPartnerLocationServiceStatuses { get; set; }

        public virtual DbSet<EventSummary> EventSummaries { get; set; }

        public virtual DbSet<EventStatus> EventStatuses { get; set; }

        public virtual DbSet<EventType> EventTypes { get; set; }

        public virtual DbSet<IftttTrigger> IftttTriggers { get; set; }

        public virtual DbSet<InvitationStatus> InvitationStatuses { get; set; }

        public virtual DbSet<MessageRequest> MessageRequests { get; set; }

        public virtual DbSet<NonEventUserNotification> NonEventUserNotifications { get; set; }

        public virtual DbSet<PartnerAdmin> PartnerAdmins { get; set; }

        public virtual DbSet<PartnerContact> PartnerContacts { get; set; }

        public virtual DbSet<PartnerDocument> PartnerDocuments { get; set; }

        public virtual DbSet<PartnerLocation> PartnerLocations { get; set; }
        
        public virtual DbSet<PartnerLocationContact> PartnerLocationContacts { get; set; }

        public virtual DbSet<PartnerLocationService> PartnerLocationServices { get; set; }

        public virtual DbSet<PartnerRequest> PartnerRequests { get; set; }

        public virtual DbSet<PartnerRequestStatus> PartnerRequestStatus { get; set; }

        public virtual DbSet<PartnerSocialMediaAccount> PartnerSocialMediaAccounts { get; set; }

        public virtual DbSet<Partner> Partners { get; set; }

        public virtual DbSet<PartnerStatus> PartnerStatus { get; set; }

        public virtual DbSet<PartnerType> PartnerTypes { get; set; }

        public virtual DbSet<PickupLocation> PickupLocations { get; set; }

        public virtual DbSet<ServiceType> ServiceTypes { get; set; }

        public virtual DbSet<SocialMediaAccountType> SocialMediaAccountTypes { get; set; }

        public virtual DbSet<SiteMetric> SiteMetrics { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserNotification> UserNotifications { get; set; }

        public virtual DbSet<UserNotificationType> UserNotificationTypes { get; set; }

        public virtual DbSet<Waiver> WaiverStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration["TMDBServerConnectionString"], x => x.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ContactRequest>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(64);

                entity.Property(e => e.Email).HasMaxLength(64);
                 
                entity.Property(e => e.Message).HasMaxLength(2048);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.ContactRequestsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactRequests_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.ContactRequestsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactRequests_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PartnerDocument>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(e => e.Url)
                    .HasMaxLength(2048)
                    .IsRequired();

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.PartnerDocuments)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerDocuments_Partner");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerDocumentsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerDocuments_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerDocumentsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerDocuments_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PartnerContact>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(64);

                entity.Property(e => e.Phone)
                   .HasMaxLength(30);

                entity.Property(e => e.Notes)
                  .HasMaxLength(2000);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.PartnerContacts)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerContacts_Partner");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerContactsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerContacts_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerContactsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerContacts_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PartnerSocialMediaAccount>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AccountIdentifier).HasMaxLength(128);

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.PartnerSocialMediaAccounts)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerSocialMediaAccount_Partner");

                entity.HasOne(d => d.SocialMediaAccountType)
                    .WithMany(p => p.PartnerSocialMediaAccounts)
                    .HasForeignKey(d => d.SocialMediaAccountTypeId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PartnerSocialMediaAccounts_SocialMediaAccountType");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerSocialMediaAccountsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerSocialMediaAccount_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerSocialMediaAccountsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerSocialMediaAccount_LastUpdatedByUser_Id");
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

                entity.Property(e => e.LastUpdatedByUserId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PostalCode)
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
                entity.HasKey(e => new { e.EventId, e.UserId });

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.Event)
                    .WithMany(d => d.EventAttendees)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendees_Events");

                entity.HasOne(d => d.User)
                    .WithMany(d => d.EventAttendees)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendees_ApplicationUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventAttendeesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendees_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventAttendeesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendees_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EventAttendeeRoute>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.Event)
                    .WithMany(d => d.EventAttendeeRoutes)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeRoutes_Events");

                entity.HasOne(d => d.User)
                    .WithMany(d => d.EventAttendeeRoutes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeRoutes_ApplicationUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventAttendeeRoutesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeRoutes_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventAttendeeRoutesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeRoutes_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EventPartnerLocationService>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.PartnerLocationId, e.ServiceTypeId });

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.PartnerLocationId)
                    .IsRequired();

                entity.Property(e => e.ServiceTypeId)
                    .IsRequired();

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartnerLocationServices_Events");

                entity.HasOne(d => d.PartnerLocation)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartnerLocationServices_PartnerLocations");

                entity.HasOne(d => d.ServiceType)
                    .WithMany()
                    .HasForeignKey(d => d.ServiceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartnerLocationServices_ServiceTypes");

                entity.HasOne(d => d.EventPartnerLocationServiceStatus)
                    .WithMany(p => p.EventPartnerLocationServices)
                    .HasForeignKey(d => d.EventPartnerLocationServiceStatusId)
                    .HasConstraintName("FK_EventPartnerLocationServices_EventPartnerLocationStatuses");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventPartnerLocationsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartnerLocationServices_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventPartnerLocationsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartnerLocationServices_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EventPartnerLocationServiceStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new EventPartnerLocationServiceStatus { Id = (int)EventPartnerLocationServiceStatusEnum.None, Name = "None", Description = "Partner has not been contacted", DisplayOrder = 1, IsActive = true },
                    new EventPartnerLocationServiceStatus { Id = (int)EventPartnerLocationServiceStatusEnum.Requested, Name = "Requested", Description = "Request is awaiting processing by partner", DisplayOrder = 2, IsActive = true },
                    new EventPartnerLocationServiceStatus { Id = (int)EventPartnerLocationServiceStatusEnum.Accepted, Name = "Accepted", Description = "Request has been approved by partner", DisplayOrder = 3, IsActive = true },
                    new EventPartnerLocationServiceStatus { Id = (int)EventPartnerLocationServiceStatusEnum.Declined, Name = "Declined", Description = "Request has been declined by partner", DisplayOrder = 4, IsActive = true });
            });

            modelBuilder.Entity<EventStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new EventStatus { Id = (int)EventStatusEnum.Active, Name = "Active", Description = "Event is actively recruiting new members", DisplayOrder = 1, IsActive = true },
                    new EventStatus { Id = (int)EventStatusEnum.Full, Name = "Full", Description = "Event is full", DisplayOrder = 2, IsActive = true },
                    new EventStatus { Id = (int)EventStatusEnum.Canceled, Name = "Canceled", Description = "Event has been canceled", DisplayOrder = 3, IsActive = true },
                    new EventStatus { Id = (int)EventStatusEnum.Complete, Name = "Completed", Description = "Event has completed", DisplayOrder = 4, IsActive = true });
            });

            modelBuilder.Entity<IftttTrigger>(entity =>
            {
                entity.HasKey(e => new { e.TriggerId });

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.IftttTriggersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IftttTriggers_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.IftttTriggersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IffttTriggers_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<InvitationStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new InvitationStatus { Id = (int)InvitationStatusEnum.New, Name = "New", Description = "Invitation has not yet been sent", DisplayOrder = 1, IsActive = true },
                    new InvitationStatus { Id = (int)InvitationStatusEnum.Sent, Name = "Sent", Description = "Invitation has been sent", DisplayOrder = 2, IsActive = true },
                    new InvitationStatus { Id = (int)InvitationStatusEnum.Accepted, Name = "Accepted", Description = "Invitation has been accepted", DisplayOrder = 3, IsActive = true },
                    new InvitationStatus { Id = (int)InvitationStatusEnum.Canceled, Name = "Canceled", Description = "Invitation has been canceled", DisplayOrder = 4, IsActive = true });
            });

            modelBuilder.Entity<EventSummary>(entity =>
            {
                entity.HasKey(e => new { e.EventId });

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.Notes).HasMaxLength(2048);

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventSummary_Events");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventSummariesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventSummaries_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventSummariesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventSummaries_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EventType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new EventType { Id = 1, Name = "Park Cleanup", Description = "Park Cleanup", DisplayOrder = 1, IsActive = true },
                    new EventType { Id = 2, Name = "School Cleanup", Description = "School Cleanup", DisplayOrder = 3, IsActive = true },
                    new EventType { Id = 3, Name = "Neighborhood Cleanup", Description = "Neighborhood Cleanup", DisplayOrder = 4, IsActive = true },
                    new EventType { Id = 4, Name = "Beach Cleanup", Description = "Beach Cleanup", DisplayOrder = 5, IsActive = true },
                    new EventType { Id = 5, Name = "Highway Cleanup", Description = "Highway Cleanup", DisplayOrder = 6, IsActive = true },
                    new EventType { Id = 6, Name = "Natural Disaster Cleanup", Description = "Natural Disaster Cleanup", DisplayOrder = 14, IsActive = true },
                    new EventType { Id = 7, Name = "Trail Cleanup", Description = "Trail Cleanup", DisplayOrder = 7, IsActive = true },
                    new EventType { Id = 8, Name = "Reef Cleanup", Description = "Reef Cleanup", DisplayOrder = 8, IsActive = true },
                    new EventType { Id = 9, Name = "Private Land Cleanup", Description = "Private Land Cleanup", DisplayOrder = 9, IsActive = true },
                    new EventType { Id = 10, Name = "Dog Park Cleanup", Description = "Dog Park Cleanup", DisplayOrder = 10, IsActive = true },
                    new EventType { Id = 11, Name = "Waterway Cleanup", Description = "Waterway Cleanup", DisplayOrder = 11, IsActive = true },
                    new EventType { Id = 12, Name = "Vandalism Cleanup", Description = "Vandalism Cleanup", DisplayOrder = 12, IsActive = true },
                    new EventType { Id = 13, Name = "Social Event", Description = "Social Event", DisplayOrder = 13, IsActive = true },
                    new EventType { Id = 14, Name = "Other", Description = "Other", DisplayOrder = 16, IsActive = true },
                    new EventType { Id = 15, Name = "Snow Removal", Description = "Snow Removal", DisplayOrder = 15, IsActive = true },
                    new EventType { Id = 16, Name = "Streetside Cleanup", Description = "Streetside Cleanup", DisplayOrder = 2, IsActive = true },
                    new EventType { Id = 17, Name = "Habitat Restoration", Description = "Habitat Restoration", DisplayOrder = 17, IsActive = true });
        });

            modelBuilder.Entity<MessageRequest>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(64);

                entity.Property(e => e.Message).HasMaxLength(2048);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.MessageRequestsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MessageRequests_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.MessageRequestsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MessageRequests_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<NonEventUserNotification>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.UserNotificationType)
                    .WithMany(p => p.NonEventUserNotifications)
                    .HasForeignKey(d => d.UserNotificationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NonEventUserNotifications_UserNotificationTypes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NonEventUserNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NonEventUserNotifications_User_Id");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.NonEventUserNotificationsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NonEventUserNotification_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.NonEventUserNotificationsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NonEventUserNotification_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PartnerTypeId).HasDefaultValue(2);

                entity.Property(e => e.PublicNotes).HasMaxLength(2048);

                entity.Property(e => e.PrivateNotes).HasMaxLength(2048);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Partners_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Partners_LastUpdatedByUser_Id");

                entity.HasOne(d => d.PartnerStatus)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.PartnerStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Partners_PartnerStatus");

                entity.HasOne(d => d.PartnerType)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.PartnerTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Partners_PartnerType");
            });

            modelBuilder.Entity<PartnerLocation>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.Property(e => e.PartnerId)
                    .IsRequired();

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PublicNotes).HasMaxLength(2048);

                entity.Property(e => e.PrivateNotes).HasMaxLength(2048);

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.PartnerLocations)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerLocations_Partners");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerLocationsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerLocations_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerLocationsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerLocations_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<PartnerLocationContact>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(64);

                entity.Property(e => e.Phone)
                   .HasMaxLength(30);

                entity.Property(e => e.Notes)
                  .HasMaxLength(2000);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.HasOne(d => d.PartnerLocation)
                    .WithMany(d => d.PartnerLocationContacts)
                    .HasForeignKey(d => d.PartnerLocationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerLocationContacts_PartnerLocation");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerLocationContactsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerLocationContacts_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerLocationContactsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerLocationContacts_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PartnerLocationService>(entity =>
            {
                entity.HasKey(e => new { e.PartnerLocationId, e.ServiceTypeId, });

                entity.Property(e => e.ServiceTypeId)
                    .IsRequired();

                entity.Property(e => e.PartnerLocationId)
                    .IsRequired();

                entity.HasOne(d => d.ServiceType)
                    .WithMany()
                    .HasForeignKey(d => d.ServiceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerLocationService_ServiceTypes");

                entity.HasOne(d => d.PartnerLocation)
                    .WithMany(d => d.PartnerLocationServices)
                    .HasForeignKey(d => d.PartnerLocationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnersLocationService_PartnerLocations");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerLocationServicesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnersLocationServices_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerLocationServicesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerLocationServices_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PartnerRequest>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.Email).HasMaxLength(64);

                entity.Property(e => e.Phone).HasMaxLength(32);

                entity.Property(e => e.Notes).HasMaxLength(2048);

                entity.Property(e => e.PartnerTypeId).HasDefaultValue(2);

                entity.Property(e => e.City)
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .HasMaxLength(64);

                entity.Property(e => e.Region)
                    .HasMaxLength(256);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(25);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerRequestsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerRequests_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerRequestsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerRequests_LastUpdatedByUser_Id");

                entity.HasOne(d => d.PartnerRequestStatus)
                    .WithMany(p => p.PartnerRequests)
                    .HasForeignKey(d => d.PartnerRequestStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerRequests_PartnerRequestStatus");

                entity.HasOne(d => d.PartnerType)
                    .WithMany(p => p.PartnerRequests)
                    .HasForeignKey(d => d.PartnerTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerRequests_PartnerType");
            });

            modelBuilder.Entity<PartnerRequestStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Sent, Name = "Invitation Sent", Description = "Invitiation has been sent", DisplayOrder = 1, IsActive = true },
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Approved, Name = "Approved", Description = "Request has been approved by the Site Administrator", DisplayOrder = 2, IsActive = true },
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Denied, Name = "Denied", Description = "Request has been approved by the Site Administrator", DisplayOrder = 3, IsActive = true },
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Pending, Name = "Pending Approval", Description = "Invitiation is pending approval by TrshMob.eco admin", DisplayOrder = 4, IsActive = true });
            });

            modelBuilder.Entity<PartnerStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new PartnerStatus { Id = (int)PartnerStatusEnum.Active, Name = "Active", Description = "Partner is Active", DisplayOrder = 1, IsActive = true },
                    new PartnerStatus { Id = (int)PartnerStatusEnum.Inactive, Name = "Inactive", Description = "Partner is Inactive", DisplayOrder = 2, IsActive = true });
            });

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new PartnerType { Id = (int)PartnerTypeEnum.Government, Name = "Government", Description = "Partner is a Government or Government Agency", DisplayOrder = 1, IsActive = true },
                    new PartnerType { Id = (int)PartnerTypeEnum.Business, Name = "Business", Description = "Partner is Business", DisplayOrder = 2, IsActive = true });
            });

            modelBuilder.Entity<PartnerAdmin>(entity =>
            {
                entity.HasKey(e => new { e.PartnerId, e.UserId });

                entity.Property(e => e.PartnerId)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.PartnerAdmins)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerAdmin_Partners");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAdmin_User");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerAdminsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAdmin_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerAdminsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAdmin_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<PartnerAdminInvitation>(entity =>
            {
                entity.HasKey(e => new { e.PartnerId, e.Email });

                entity.Property(e => e.PartnerId)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired();

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.PartnerAdminInvitations)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerAdminInvitation_Partners");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerAdminInvitationsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAdminInvitation_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerAdminInvitationsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAdminInvitation_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<PickupLocation>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.Notes).HasMaxLength(2048);

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasOne(d => d.Event)
                    .WithMany(d => d.PickupLocations)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PickupLocations_Events");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PickupLocationsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PickupLocations_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PickupLocationsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PickupLocations_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new ServiceType { Id = (int)ServiceTypeEnum.Hauling, Name = "Hauling", Description = "Partner will haul litter away", DisplayOrder = 1, IsActive = true },
                    new ServiceType { Id = (int)ServiceTypeEnum.DisposalLocation, Name = "Disposal Location", Description = "Partner will dispose of litter", DisplayOrder = 2, IsActive = true },
                    new ServiceType { Id = (int)ServiceTypeEnum.StartupKits, Name = "Startup Kits", Description = "Partner distributes starter kits", DisplayOrder = 3, IsActive = true },
                    new ServiceType { Id = (int)ServiceTypeEnum.Supplies, Name = "Supplies", Description = "Partner distributes supplies", DisplayOrder = 4, IsActive = true });
            });

            modelBuilder.Entity<SocialMediaAccountType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new SocialMediaAccountType { Id = (int)SocialMediaAccountTypeEnum.Facebook, Name = "Facebook", Description = "Facebook", DisplayOrder = 1 },
                    new SocialMediaAccountType { Id = (int)SocialMediaAccountTypeEnum.Twitter, Name = "Twitter", Description = "Twitter", DisplayOrder = 2 },
                    new SocialMediaAccountType { Id = (int)SocialMediaAccountTypeEnum.Instagram, Name = "Instagram", Description = "Instagram", DisplayOrder = 3 },
                    new SocialMediaAccountType { Id = (int)SocialMediaAccountTypeEnum.TikTok, Name = "TikTok", Description = "TikTok", DisplayOrder = 4 });
            });

            modelBuilder.Entity<SiteMetric>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MetricType)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.ProcessedTime)
                    .IsRequired();

                entity.Property(e => e.MetricValue)
                    .IsRequired();

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.SiteMetricsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SiteMetrics_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.SiteMetricsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SiteMetrics_LastUpdatedByUser_Id");

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => e.UserName)
                        .IsUnique();

                entity.HasIndex(e => e.Email)
                        .IsUnique();

                entity.Property(e => e.UserName).HasMaxLength(32);

                entity.Property(e => e.SourceSystemUserName).HasMaxLength(32);

                entity.Property(e => e.Email).HasMaxLength(64);

                entity.Property(e => e.City).HasMaxLength(64);

                entity.Property(e => e.PostalCode).HasMaxLength(25);

                entity.Property(e => e.TrashMobWaiverVersion).HasMaxLength(50);

                entity.HasData(
                    new User { Id = Guid.Empty, City = "Anytown", Country = "AnyCountry", Email = "info@trashmob.eco", Region = "AnyState", UserName = "TrashMob" });

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.UsersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.UsersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.UserNotificationType)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.UserNotificationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotifications_UserNotificationTypes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotifications_User_Id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotifications_Event_Id");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.UserNotificationsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotification_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.UserNotificationsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserNotification_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<UserNotificationType>(entity =>
            {
                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new UserNotificationType { Id = (int)NotificationTypeEnum.EventSummaryAttendee, Name = "EventSummaryAttendee", Description = "Opt out of Post Event Summary", DisplayOrder = 1 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.EventSummaryHostReminder, Name = "EventSummaryHostReminder", Description = "Opt out of Event Summary Reminder for events you have lead", DisplayOrder = 2 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UpcomingEventAttendingThisWeek, Name = "UpcomingEventAttendingThisWeek", Description = "Opt out of notifications for events upcoming this week you are attending", DisplayOrder = 3 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UpcomingEventAttendingSoon, Name = "UpcomingEventAttendingSoon", Description = "Opt out of notifications for events happening soon you are attending", DisplayOrder = 4 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UpcomingEventHostingThisWeek, Name = "UpcomingEventHostingThisWeek", Description = "Opt out of notifications for events upcoming this week you are leading", DisplayOrder = 5 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UpcomingEventHostingSoon, Name = "UpcomingEventHostingSoon", Description = "Opt out of notifications for events happening soon you are leading", DisplayOrder = 6 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek, Name = "UpcomingEventsInYourAreaThisWeek", Description = "Opt out of notification for new events upcoming in your area this week", DisplayOrder = 7 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UpcomingEventsInYourAreaSoon, Name = "UpcomingEventsInYourAreaSoon", Description = "Opt out of notification for new events happening in your area soon", DisplayOrder = 8 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.EventSummaryHostWeekReminder, Name = "EventSummaryHostWeekReminder", Description = "Opt out of Event Summary Week Reminder for events you have lead", DisplayOrder = 9 },
                    new UserNotificationType { Id = (int)NotificationTypeEnum.UserProfileUpdateLocation, Name = "UserProfileUpdateLocation", Description = "Opt out of notifications for User Profile Location", DisplayOrder = 10 });
            });

            modelBuilder.Entity<Waiver>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasKey(e => new { e.Name });

                entity.Property(e => e.IsWaiverEnabled)
                    .IsRequired();

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.WaiverStatusesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WaiverStatuses_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.WaiverStatusesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WaiverStatuses_User_LastUpdatedBy");

                // Stick in a default value for the Waiver
                entity.HasData(
                    new Waiver { Id = new Guid("4D222D04-AC1F-4A87-886D-FDB686F9F55C"), Name = "trashmob", IsWaiverEnabled = false, CreatedByUserId = Guid.Empty, LastUpdatedByUserId = Guid.Empty, CreatedDate = new DateTimeOffset(2022, 11, 24, 0, 0, 0, TimeSpan.Zero) });
            });

        }
    }
}
