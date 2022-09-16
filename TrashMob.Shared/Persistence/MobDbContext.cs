namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;

    public class MobDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MobDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<Community> Communities { get; set; }

        public virtual DbSet<CommunityRequest> CommunityRequests { get; set; }

        public virtual DbSet<CommunityAttachment> CommunityAttachments { get; set; }

        public virtual DbSet<CommunityContact> CommunityContacts { get; set; }

        public virtual DbSet<CommunityContactType> CommunityContactTypes { get; set; }

        public virtual DbSet<CommunityNote> CommunityNotes { get; set; }

        public virtual DbSet<CommunityPartner> CommunityPartners { get; set; }

        public virtual DbSet<CommunitySocialMediaAccount> CommunitySocialMediaAccounts { get; set; }

        public virtual DbSet<CommunityStatus> CommunityStatuses { get; set; }

        public virtual DbSet<CommunityUser> CommunityUsers { get; set; }

        public virtual DbSet<ContactRequest> ContactRequests { get; set; }

        public virtual DbSet<EventPartner> EventPartners { get; set; }

        public virtual DbSet<EventAttendee> EventAttendees { get; set; }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<EventHistory> EventHistories { get; set; }

        public virtual DbSet<EventMedia> EventMedias { get; set; }

        public virtual DbSet<EventPartnerStatus> EventPartnerStatuses { get; set; }

        public virtual DbSet<EventSummary> EventSummaries { get; set; }

        public virtual DbSet<EventStatus> EventStatuses { get; set; }

        public virtual DbSet<EventType> EventTypes { get; set; }

        public virtual DbSet<MediaType> MediaTypes { get; set; }

        public virtual DbSet<MediaType> MediaUsageTypes { get; set; }

        public virtual DbSet<MessageRequest> MessageRequests { get; set; }

        public virtual DbSet<NonEventUserNotification> NonEventUserNotifications { get; set; }

        public virtual DbSet<PartnerLocation> PartnerLocations { get; set; }

        public virtual DbSet<PartnerRequest> PartnerRequests { get; set; }

        public virtual DbSet<PartnerRequestStatus> PartnerRequestStatus { get; set; }

        public virtual DbSet<Partner> Partners { get; set; }

        public virtual DbSet<PartnerStatus> PartnerStatus { get; set; }

        public virtual DbSet<PartnerUser> PartnerUsers { get; set; }

        public virtual DbSet<SocialMediaAccount> SocialMediaAccounts { get; set; }

        public virtual DbSet<SocialMediaAccountType> SocialMediaAccountTypes { get; set; }

        public virtual DbSet<SiteMetric> SiteMetrics { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserNotification> UserNotifications { get; set; }

        public virtual DbSet<UserNotificationType> UserNotificationTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration["TMDBServerConnectionString"]);
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
            });

            modelBuilder.Entity<Community>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.City)
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(25);

                entity.HasOne(d => d.CommunityStatus)
                    .WithMany(p => p.Communities)
                    .HasForeignKey(d => d.CommunityStatusId)
                    .HasConstraintName("FK_Communities_CommunityStatuses");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunitiesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Communities_ApplicationUser_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunitiesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Communities_ApplicationUser_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityAttachment>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CommunityId);

                entity.Property(e => e.AttachmentUrl)
                    .HasMaxLength(2048)
                    .IsRequired();

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityAttachments_Community");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityAttachmentsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityAttachments_ApplicationUser_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunityAttachmentsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityAttachments_ApplicationUser_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityContact>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CommunityId);

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

                entity.HasOne(d => d.CommunityContactType)
                    .WithMany(p => p.CommunityContacts)
                            .HasForeignKey(d => d.CommunityContactTypeId)
                            .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityContacts_CommunityContactTypes");

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityContacts_Community");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityContactsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityContacts_ApplicationUser_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunityContactsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityContacts_ApplicationUser_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityContactType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new CommunityContactType { Id = (int)CommunityContactTypeEnum.None, Name = "None", Description = "Contact is not available", DisplayOrder = 1 },
                    new CommunityContactType { Id = (int)CommunityContactTypeEnum.Official, Name = "Official", Description = "Contact is an official within the Community", DisplayOrder = 2 },
                    new CommunityContactType { Id = (int)CommunityContactTypeEnum.TrashMobHeadquarters, Name = "TrashMobHQ", Description = "Contact is TrashMobHeadquarters", DisplayOrder = 3 },
                    new CommunityContactType { Id = (int)CommunityContactTypeEnum.TrashMobVolunteer, Name = "TrashMob Volunteer", Description = "Contact is a TrashMob Volunteer in the community", DisplayOrder = 4 },
                    new CommunityContactType { Id = (int)CommunityContactTypeEnum.Partner, Name = "TrashMob Partner", Description = "Contact is a TrashMob Partner in the community", DisplayOrder = 5 });
            });

            modelBuilder.Entity<CommunityNote>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CommunityId);

                entity.Property(e => e.Notes)
                  .HasMaxLength(2000);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityNotes_Community");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityNotesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityNotes_ApplicationUser_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunityNotesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityNotes_ApplicationUser_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityPartner>(entity =>
            {
                entity.HasKey(e => new { e.CommunityId, e.PartnerId, e.PartnerLocationId });

                entity.Property(e => e.CommunityId)
                    .IsRequired();

                entity.Property(e => e.PartnerId)
                    .IsRequired();

                entity.Property(e => e.PartnerLocationId)
                    .IsRequired();

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityPartners_Communities");

                entity.HasOne(d => d.Partner)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityPartners_Partners");

                entity.HasOne(d => d.PartnerLocation)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityPartners_PartnerLocations");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityPartnersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityPartners_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunityPartnersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityPartners_User_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityRequest>(entity =>
            {
                entity.Property(e => e.ContactName).HasMaxLength(128);

                entity.Property(e => e.Website).HasMaxLength(1024);

                entity.Property(e => e.Email).HasMaxLength(64);

                entity.Property(e => e.Phone).HasMaxLength(32);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityRequestsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityRequests_CreatedByUser_Id");
            });

            modelBuilder.Entity<CommunitySocialMediaAccount>(entity =>
            {
                entity.HasKey(e => new { e.CommunityId, e.SocialMediaAccountId, });

                entity.Property(e => e.SocialMediaAccountId)
                    .IsRequired();

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunitySocialMediaAccount_Communities");

                entity.HasOne(d => d.SocialMediaAccount)
                    .WithMany()
                    .HasForeignKey(d => d.SocialMediaAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunitySocialMediaAccount_SocialMediaAccount");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunitySocialMediaAccountsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunitySocialMediaAccount_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunitySocialMediaAccountsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunitySocialMediaAccount_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<CommunityStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new CommunityStatus { Id = (int)CommunityStatusEnum.Active, Name = "Active", Description = "Community is an active TrashMob community", DisplayOrder = 1, IsActive = true },
                    new CommunityStatus { Id = (int)CommunityStatusEnum.Inactive, Name = "Inactive", Description = "Community is not currently an active TrashMob community", DisplayOrder = 2, IsActive = true });
            });

            modelBuilder.Entity<CommunityUser>(entity =>
            {
                entity.HasKey(e => new { e.CommunityId, e.UserId, });

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityUser_Communities");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityUser_User");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityUsersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityUsers_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunityUsersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityUsers_LastUpdatedByUser_Id");
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
                entity.ToTable("EventHistory");

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
            });

            modelBuilder.Entity<EventMedia>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MediaUrl).IsRequired();

                entity.HasOne(d => d.MediaType)
                    .WithMany(p => p.EventMedias)
                    .HasForeignKey(d => d.MediaTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventMedias_MediaTypes");

                entity.HasOne(d => d.MediaUsageType)
                    .WithMany(p => p.EventMedias)
                    .HasForeignKey(d => d.MediaUsageTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventMedias_MediaUsageTypes");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventMedias)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventMedias_User_Id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventMedias)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventMedia_Event_Id");
            });

            modelBuilder.Entity<EventPartner>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.PartnerId, e.PartnerLocationId });

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.PartnerId)
                    .IsRequired();

                entity.Property(e => e.PartnerLocationId)
                    .IsRequired();

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartners_Events");

                entity.HasOne(d => d.Partner)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartners_Partners");

                entity.HasOne(d => d.PartnerLocation)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartners_PartnerLocations");

                entity.HasOne(d => d.EventPartnerStatus)
                    .WithMany(p => p.EventPartners)
                    .HasForeignKey(d => d.EventPartnerStatusId)
                    .HasConstraintName("FK_EventPartners_EventPartnerStatuses");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventPartnersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartners_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventPartnersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPartners_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EventPartnerStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new EventPartnerStatus { Id = (int)EventPartnerStatusEnum.None, Name = "None", Description = "Partner has not been contacted", DisplayOrder = 1, IsActive = true },
                    new EventPartnerStatus { Id = (int)EventPartnerStatusEnum.Requested, Name = "Requested", Description = "Request is awaiting processing by partner", DisplayOrder = 2, IsActive = true },
                    new EventPartnerStatus { Id = (int)EventPartnerStatusEnum.Accepted, Name = "Accepted", Description = "Request has been approved by partner", DisplayOrder = 3, IsActive = true },
                    new EventPartnerStatus { Id = (int)EventPartnerStatusEnum.Declined, Name = "Declined", Description = "Request has been declined by partner", DisplayOrder = 4, IsActive = true });
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
                    new EventType { Id = 2, Name = "School Cleanup", Description = "School Cleanup", DisplayOrder = 2, IsActive = true },
                    new EventType { Id = 3, Name = "Neighborhood Cleanup", Description = "Neighborhood Cleanup", DisplayOrder = 3, IsActive = true },
                    new EventType { Id = 4, Name = "Beach Cleanup", Description = "Beach Cleanup", DisplayOrder = 4, IsActive = true },
                    new EventType { Id = 5, Name = "Highway Cleanup", Description = "Highway Cleanup", DisplayOrder = 5, IsActive = true },
                    new EventType { Id = 6, Name = "Natural Disaster Cleanup", Description = "Natural Disaster Cleanup", DisplayOrder = 6, IsActive = true },
                    new EventType { Id = 7, Name = "Trail Cleanup", Description = "Trail Cleanup", DisplayOrder = 7, IsActive = true },
                    new EventType { Id = 8, Name = "Reef Cleanup", Description = "Reef Cleanup", DisplayOrder = 8, IsActive = true },
                    new EventType { Id = 9, Name = "Private Land Cleanup", Description = "Private Land Cleanup", DisplayOrder = 9, IsActive = true },
                    new EventType { Id = 10, Name = "Dog Park Cleanup", Description = "Dog Park Cleanup", DisplayOrder = 10, IsActive = true },
                    new EventType { Id = 11, Name = "Waterway Cleanup", Description = "Waterway Cleanup", DisplayOrder = 11, IsActive = true },
                    new EventType { Id = 12, Name = "Vandalism Cleanup", Description = "Vandalism Cleanup", DisplayOrder = 12, IsActive = true },
                    new EventType { Id = 13, Name = "Social Event", Description = "Social Event", DisplayOrder = 13, IsActive = true },
                    new EventType { Id = 14, Name = "Other", Description = "Other", DisplayOrder = 14, IsActive = true });
            });

            modelBuilder.Entity<MediaType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new MediaType { Id = 1, Name = "Instagram", Description = "Instagram Image or Video", DisplayOrder = 1, IsActive = true },
                    new MediaType { Id = 2, Name = "YouTube", Description = "YouTube Video", DisplayOrder = 2, IsActive = true });
            });

            modelBuilder.Entity<MediaUsageType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new MediaUsageType { Id = 1, Name = "BeforeEvent", Description = "Before a cleanup event", DisplayOrder = 1, IsActive = true },
                    new MediaUsageType { Id = 2, Name = "AfterEvent", Description = "After a cleanup event", DisplayOrder = 2, IsActive = true });

            });

            modelBuilder.Entity<MessageRequest>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(64);

                entity.Property(e => e.Message).HasMaxLength(2048);
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
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PrimaryEmail).HasMaxLength(64);

                entity.Property(e => e.SecondaryEmail).HasMaxLength(64);

                entity.Property(e => e.PrimaryPhone).HasMaxLength(32);

                entity.Property(e => e.SecondaryPhone).HasMaxLength(32);

                entity.Property(e => e.Notes).HasMaxLength(2048);

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
                    .HasConstraintName("FK_Partners_PartnerRequestStatus");
            });

            modelBuilder.Entity<PartnerLocation>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.Property(e => e.PartnerId)
                    .IsRequired();

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PrimaryEmail).HasMaxLength(64);

                entity.Property(e => e.SecondaryEmail).HasMaxLength(64);

                entity.Property(e => e.PrimaryPhone).HasMaxLength(32);

                entity.Property(e => e.SecondaryPhone).HasMaxLength(32);

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

                entity.HasOne(d => d.Partner)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
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

            modelBuilder.Entity<PartnerRequest>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PrimaryEmail).HasMaxLength(64);

                entity.Property(e => e.SecondaryEmail).HasMaxLength(64);

                entity.Property(e => e.PrimaryPhone).HasMaxLength(32);

                entity.Property(e => e.SecondaryPhone).HasMaxLength(32);

                entity.Property(e => e.Notes).HasMaxLength(2048);

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
            });

            modelBuilder.Entity<PartnerRequestStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Pending, Name = "Pending", Description = "Request is Pending Approval by Site Administrator", DisplayOrder = 1, IsActive = true },
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Approved, Name = "Approved", Description = "Request has been approved by the Site Administrator", DisplayOrder = 2, IsActive = true },
                    new PartnerRequestStatus { Id = (int)PartnerRequestStatusEnum.Denied, Name = "Denied", Description = "Request has been approved by the Site Administrator", DisplayOrder = 3, IsActive = true });
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

            modelBuilder.Entity<PartnerUser>(entity =>
            {
                entity.HasKey(e => new { e.PartnerId, e.UserId, });

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.HasOne(d => d.Partner)
                    .WithMany()
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerUser_Partners");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerUser_User");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerUsersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerUsers_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerUsersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerUsers_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<SocialMediaAccount>(entity =>
            {
                entity.Property(e => e.AccountIdentifier).HasMaxLength(128);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.SocialMediaAccountsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SocialMediaAccount_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.SocialMediaAccountsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SocialMediaAccount_LastUpdatedByUser_Id");
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
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserName).HasMaxLength(32);

                entity.Property(e => e.SourceSystemUserName).HasMaxLength(32);

                entity.Property(e => e.GivenName).HasMaxLength(32);

                entity.Property(e => e.SurName).HasMaxLength(32);

                entity.Property(e => e.Email).HasMaxLength(64);

                entity.Property(e => e.City).HasMaxLength(64);

                entity.Property(e => e.PostalCode).HasMaxLength(25);

                entity.Property(e => e.PrivacyPolicyVersion).HasMaxLength(50);

                entity.Property(e => e.PrivacyPolicyVersion).HasMaxLength(50);

                entity.Property(e => e.TermsOfServiceVersion).HasMaxLength(50);

                entity.Property(e => e.TrashMobWaiverVersion).HasMaxLength(50);

                entity.HasData(
                    new User { Id = Guid.Empty, City = "Anytown", Country = "AnyCountry", Email = "info@trashmob.eco", GivenName = "TrashMob", Region = "AnyState", SurName = "Eco", UserName = "TrashMob" });
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
        }
    }
}
