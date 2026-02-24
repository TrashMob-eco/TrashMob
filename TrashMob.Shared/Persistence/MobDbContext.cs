namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;

    /// <summary>
    /// Provides the Entity Framework database context for the TrashMob application.
    /// </summary>
    public class MobDbContext(IConfiguration configuration) : DbContext
    {

        public virtual DbSet<ContactRequest> ContactRequests { get; set; }

        public virtual DbSet<EventPartnerLocationService> EventPartnerLocationServices { get; set; }

        public virtual DbSet<EventAttendee> EventAttendees { get; set; }

        public virtual DbSet<EventAttendeeRoute> EventAttendeeRoutes { get; set; }

        public virtual DbSet<RoutePoint> RoutePoints { get; set; }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<EventPartnerLocationServiceStatus> EventPartnerLocationServiceStatuses { get; set; }

        public virtual DbSet<EventSummary> EventSummaries { get; set; }

        public virtual DbSet<EventStatus> EventStatuses { get; set; }

        public virtual DbSet<EventType> EventTypes { get; set; }

        public virtual DbSet<IftttTrigger> IftttTriggers { get; set; }

        public virtual DbSet<InvitationStatus> InvitationStatuses { get; set; }

        public virtual DbSet<JobOpportunity> JobOpportunities { get; set; }

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

        public virtual DbSet<AdoptableArea> AdoptableAreas { get; set; }

        public virtual DbSet<AreaGenerationBatch> AreaGenerationBatches { get; set; }

        public virtual DbSet<StagedAdoptableArea> StagedAdoptableAreas { get; set; }

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

        public virtual DbSet<WaiverVersion> WaiverVersions { get; set; }

        public virtual DbSet<CommunityWaiver> CommunityWaivers { get; set; }

        public virtual DbSet<UserWaiver> UserWaivers { get; set; }

        public virtual DbSet<WeightUnit> WeightUnits { get; set; }

        public virtual DbSet<LitterReportStatus> LitterReportStatuses { get; set; }

        public virtual DbSet<EventLitterReport> EventLitterReports { get; set; }

        public virtual DbSet<LitterImage> LitterImages { get; set; }

        public virtual DbSet<LitterReport> LitterReports { get; set; }

        public virtual DbSet<Team> Teams { get; set; }

        public virtual DbSet<TeamMember> TeamMembers { get; set; }

        public virtual DbSet<TeamJoinRequest> TeamJoinRequests { get; set; }

        public virtual DbSet<TeamAdoption> TeamAdoptions { get; set; }

        public virtual DbSet<TeamAdoptionEvent> TeamAdoptionEvents { get; set; }

        public virtual DbSet<Sponsor> Sponsors { get; set; }

        public virtual DbSet<ProfessionalCompany> ProfessionalCompanies { get; set; }

        public virtual DbSet<ProfessionalCompanyUser> ProfessionalCompanyUsers { get; set; }

        public virtual DbSet<SponsoredAdoption> SponsoredAdoptions { get; set; }

        public virtual DbSet<ProfessionalCleanupLog> ProfessionalCleanupLogs { get; set; }

        public virtual DbSet<TeamEvent> TeamEvents { get; set; }

        public virtual DbSet<TeamPhoto> TeamPhotos { get; set; }

        public virtual DbSet<PartnerPhoto> PartnerPhotos { get; set; }

        public virtual DbSet<UserFeedback> UserFeedback { get; set; }

        public virtual DbSet<PhotoFlag> PhotoFlags { get; set; }

        public virtual DbSet<PhotoModerationLog> PhotoModerationLogs { get; set; }

        public virtual DbSet<EventAttendeeMetrics> EventAttendeeMetrics { get; set; }

        public virtual DbSet<EmailInviteBatch> EmailInviteBatches { get; set; }

        public virtual DbSet<EmailInvite> EmailInvites { get; set; }

        public virtual DbSet<LeaderboardCache> LeaderboardCaches { get; set; }

        public virtual DbSet<NewsletterCategory> NewsletterCategories { get; set; }

        public virtual DbSet<NewsletterTemplate> NewsletterTemplates { get; set; }

        public virtual DbSet<Newsletter> Newsletters { get; set; }

        public virtual DbSet<UserNewsletterPreference> UserNewsletterPreferences { get; set; }

        public virtual DbSet<AchievementType> AchievementTypes { get; set; }

        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        public virtual DbSet<EventPhoto> EventPhotos { get; set; }

        public virtual DbSet<CommunityProspect> CommunityProspects { get; set; }

        public virtual DbSet<ProspectActivity> ProspectActivities { get; set; }

        public virtual DbSet<ProspectOutreachEmail> ProspectOutreachEmails { get; set; }

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

            modelBuilder.Entity<JobOpportunity>(entity =>
            {
                entity.ToTable("JobOpportunities");

                entity.Property(e => e.Title).HasMaxLength(150);

                entity.Property(e => e.TagLine).HasMaxLength(250);

                entity.Property(e => e.FullDescription).HasMaxLength(5000);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.JobOpportunitiesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobOpportunities_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.JobOpportunitiesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobOpportunities_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PartnerDocument>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(e => e.Url)
                    .HasMaxLength(2048);

                entity.Property(e => e.BlobStoragePath)
                    .HasMaxLength(1024);

                entity.Property(e => e.ContentType)
                    .HasMaxLength(256);

                entity.Property(e => e.FileSizeBytes);

                entity.Property(e => e.DocumentTypeId)
                    .HasDefaultValue(0);

                entity.Property(e => e.ExpirationDate);

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
                    .HasMaxLength(256);

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

                entity.HasOne(d => d.Team)
                    .WithMany()
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Events_Teams_TeamId");

                entity.HasIndex(e => e.EventVisibilityId)
                    .HasDatabaseName("IX_Events_EventVisibilityId");

                entity.HasIndex(e => e.TeamId)
                    .HasDatabaseName("IX_Events_TeamId");

                entity.Ignore(e => e.IsEventPublic);
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

                entity.HasIndex(e => new { e.EventId, e.IsEventLead })
                    .HasDatabaseName("IX_EventAttendees_EventId_IsEventLead");
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

                entity.Property(e => e.PrivacyLevel)
                    .HasMaxLength(20)
                    .HasDefaultValue("EventOnly");

                entity.Property(e => e.TrimStartMeters)
                    .HasDefaultValue(100);

                entity.Property(e => e.TrimEndMeters)
                    .HasDefaultValue(100);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.IsTimeTrimmed)
                    .HasDefaultValue(false);

                entity.HasOne<WeightUnit>()
                    .WithMany()
                    .HasForeignKey(e => e.WeightUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeRoutes_WeightUnits");

                entity.HasMany(e => e.RoutePoints)
                    .WithOne(rp => rp.Route)
                    .HasForeignKey(rp => rp.RouteId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoutePoints_EventAttendeeRoutes");
            });

            modelBuilder.Entity<RoutePoint>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.RouteId)
                    .IsRequired();

                entity.Property(e => e.Latitude)
                    .IsRequired();

                entity.Property(e => e.Longitude)
                    .IsRequired();

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                entity.HasIndex(e => e.RouteId)
                    .HasDatabaseName("IX_RoutePoints_RouteId");

                entity.HasIndex(e => new { e.RouteId, e.Timestamp })
                    .HasDatabaseName("IX_RoutePoints_RouteId_Timestamp");
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
                    new EventPartnerLocationServiceStatus
                    {
                        Id = (int)EventPartnerLocationServiceStatusEnum.None, Name = "None",
                        Description = "Partner has not been contacted", DisplayOrder = 1, IsActive = true,
                    },
                    new EventPartnerLocationServiceStatus
                    {
                        Id = (int)EventPartnerLocationServiceStatusEnum.Requested, Name = "Requested",
                        Description = "Request is awaiting processing by partner", DisplayOrder = 2, IsActive = true,
                    },
                    new EventPartnerLocationServiceStatus
                    {
                        Id = (int)EventPartnerLocationServiceStatusEnum.Accepted, Name = "Accepted",
                        Description = "Request has been approved by partner", DisplayOrder = 3, IsActive = true,
                    },
                    new EventPartnerLocationServiceStatus
                    {
                        Id = (int)EventPartnerLocationServiceStatusEnum.Declined, Name = "Declined",
                        Description = "Request has been declined by partner", DisplayOrder = 4, IsActive = true,
                    });
            });

            modelBuilder.Entity<EventStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new EventStatus
                    {
                        Id = (int)EventStatusEnum.Active, Name = "Active",
                        Description = "Event is actively recruiting new members", DisplayOrder = 1, IsActive = true,
                    },
                    new EventStatus
                    {
                        Id = (int)EventStatusEnum.Full, Name = "Full", Description = "Event is full", DisplayOrder = 2,
                        IsActive = true,
                    },
                    new EventStatus
                    {
                        Id = (int)EventStatusEnum.Canceled, Name = "Canceled", Description = "Event has been canceled",
                        DisplayOrder = 3, IsActive = true,
                    },
                    new EventStatus
                    {
                        Id = (int)EventStatusEnum.Complete, Name = "Completed", Description = "Event has completed",
                        DisplayOrder = 4, IsActive = true,
                    });
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
                    new InvitationStatus
                    {
                        Id = (int)InvitationStatusEnum.New, Name = "New",
                        Description = "Invitation has not yet been sent", DisplayOrder = 1, IsActive = true,
                    },
                    new InvitationStatus
                    {
                        Id = (int)InvitationStatusEnum.Sent, Name = "Sent", Description = "Invitation has been sent",
                        DisplayOrder = 2, IsActive = true,
                    },
                    new InvitationStatus
                    {
                        Id = (int)InvitationStatusEnum.Accepted, Name = "Accepted",
                        Description = "Invitation has been accepted", DisplayOrder = 3, IsActive = true,
                    },
                    new InvitationStatus
                    {
                        Id = (int)InvitationStatusEnum.Canceled, Name = "Canceled",
                        Description = "Invitation has been canceled", DisplayOrder = 4, IsActive = true,
                    });
            });

            modelBuilder.Entity<EventSummary>(entity =>
            {
                entity.HasKey(e => new { e.EventId });

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.Notes).HasMaxLength(2048);

                entity.Property(e => e.IsFromRouteData).HasDefaultValue(false);

                entity.Property(e => e.PickedWeightUnitId).HasDefaultValue(0);

                entity.Property(e => e.PickedWeight).HasPrecision(10, 1);

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventSummary_Events");

                entity.HasOne(d => d.PickedWeightUnit)
                    .WithMany()
                    .HasForeignKey(d => d.PickedWeightUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventSummary_PickedWeightUnits");

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
                    new EventType
                    {
                        Id = 1, Name = "Park Cleanup", Description = "Park Cleanup", DisplayOrder = 1, IsActive = true,
                    },
                    new EventType
                    {
                        Id = 2, Name = "School Cleanup", Description = "School Cleanup", DisplayOrder = 3,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 3, Name = "Neighborhood Cleanup", Description = "Neighborhood Cleanup", DisplayOrder = 4,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 4, Name = "Beach Cleanup", Description = "Beach Cleanup", DisplayOrder = 5, IsActive = true,
                    },
                    new EventType
                    {
                        Id = 5, Name = "Highway Cleanup", Description = "Highway Cleanup", DisplayOrder = 6,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 6, Name = "Natural Disaster Cleanup", Description = "Natural Disaster Cleanup",
                        DisplayOrder = 14, IsActive = true,
                    },
                    new EventType
                    {
                        Id = 7, Name = "Trail Cleanup", Description = "Trail Cleanup", DisplayOrder = 7, IsActive = true,
                    },
                    new EventType
                    {
                        Id = 8, Name = "Reef Cleanup", Description = "Reef Cleanup", DisplayOrder = 8, IsActive = true,
                    },
                    new EventType
                    {
                        Id = 9, Name = "Private Land Cleanup", Description = "Private Land Cleanup", DisplayOrder = 9,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 10, Name = "Dog Park Cleanup", Description = "Dog Park Cleanup", DisplayOrder = 10,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 11, Name = "Waterway Cleanup", Description = "Waterway Cleanup", DisplayOrder = 11,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 12, Name = "Vandalism Cleanup", Description = "Vandalism Cleanup", DisplayOrder = 12,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 13, Name = "Social Event", Description = "Social Event", DisplayOrder = 13, IsActive = true,
                    },
                    new EventType
                    { Id = 14, Name = "Other", Description = "Other", DisplayOrder = 16, IsActive = true },
                    new EventType
                    {
                        Id = 15, Name = "Snow Removal", Description = "Snow Removal", DisplayOrder = 15, IsActive = true,
                    },
                    new EventType
                    {
                        Id = 16, Name = "Streetside Cleanup", Description = "Streetside Cleanup", DisplayOrder = 2,
                        IsActive = true,
                    },
                    new EventType
                    {
                        Id = 17, Name = "Habitat Restoration", Description = "Habitat Restoration", DisplayOrder = 17,
                        IsActive = true,
                    });
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

                // Community Home Page Properties
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.HasIndex(e => e.Slug)
                    .IsUnique()
                    .HasFilter("[Slug] IS NOT NULL")
                    .HasDatabaseName("IX_Partners_Slug");
                entity.Property(e => e.HomePageEnabled).HasDefaultValue(false);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.BrandingPrimaryColor).HasMaxLength(7);
                entity.Property(e => e.BrandingSecondaryColor).HasMaxLength(7);
                entity.Property(e => e.BannerImageUrl).HasMaxLength(500);
                entity.Property(e => e.Tagline).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(256);
                entity.Property(e => e.Region).HasMaxLength(256);
                entity.Property(e => e.Country).HasMaxLength(64);
                entity.Property(e => e.CountyName).HasMaxLength(256);
                entity.Property(e => e.LogoUrl).HasMaxLength(500);
                entity.Property(e => e.ContactEmail).HasMaxLength(256);
                entity.Property(e => e.ContactPhone).HasMaxLength(50);
                entity.Property(e => e.PhysicalAddress).HasMaxLength(500);

                // Boundary GeoJSON (can be large – use nvarchar(max))
                entity.Property(e => e.BoundaryGeoJson).HasColumnType("nvarchar(max)");

                // Adoptable Area Defaults
                entity.Property(e => e.DefaultSafetyRequirements).HasMaxLength(4000);

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

            modelBuilder.Entity<AdoptableArea>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(2048);

                entity.Property(e => e.AreaType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Available");

                entity.Property(e => e.SafetyRequirements)
                    .HasMaxLength(4000);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.AllowCoAdoption)
                    .HasDefaultValue(false);

                entity.Property(e => e.CleanupFrequencyDays)
                    .HasDefaultValue(90);

                entity.Property(e => e.MinEventsPerYear)
                    .HasDefaultValue(4);

                entity.HasOne(e => e.Partner)
                    .WithMany(p => p.AdoptableAreas)
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AdoptableAreas_Partners");

                entity.HasIndex(e => e.PartnerId)
                    .HasDatabaseName("IX_AdoptableAreas_PartnerId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_AdoptableAreas_Status");

                entity.HasIndex(e => new { e.PartnerId, e.Name })
                    .IsUnique()
                    .HasDatabaseName("IX_AdoptableAreas_PartnerId_Name");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdoptableAreas_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdoptableAreas_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<AreaGenerationBatch>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Queued");

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(4000);

                entity.HasOne(e => e.Partner)
                    .WithMany()
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AreaGenerationBatches_Partners");

                entity.HasIndex(e => new { e.PartnerId, e.Status })
                    .HasDatabaseName("IX_AreaGenerationBatches_PartnerId_Status");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AreaGenerationBatches_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AreaGenerationBatches_LastUpdatedByUser_Id");
            });

            modelBuilder.Entity<StagedAdoptableArea>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(2048);

                entity.Property(e => e.AreaType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ReviewStatus)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.Confidence)
                    .HasMaxLength(20)
                    .HasDefaultValue("Medium");

                entity.Property(e => e.DuplicateOfName)
                    .HasMaxLength(200);

                entity.Property(e => e.OsmId)
                    .HasMaxLength(50);

                entity.Property(e => e.OsmTags)
                    .HasMaxLength(4000);

                entity.HasOne(e => e.Batch)
                    .WithMany(b => b.StagedAreas)
                    .HasForeignKey(e => e.BatchId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_StagedAdoptableAreas_AreaGenerationBatches");

                entity.HasOne(e => e.Partner)
                    .WithMany()
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_StagedAdoptableAreas_Partners");

                entity.HasIndex(e => new { e.BatchId, e.ReviewStatus })
                    .HasDatabaseName("IX_StagedAdoptableAreas_BatchId_ReviewStatus");

                entity.HasIndex(e => e.PartnerId)
                    .HasDatabaseName("IX_StagedAdoptableAreas_PartnerId");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StagedAdoptableAreas_CreatedByUser_Id");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StagedAdoptableAreas_LastUpdatedByUser_Id");
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
                entity.HasKey(e => new { e.PartnerLocationId, e.ServiceTypeId });

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
                    new PartnerRequestStatus
                    {
                        Id = (int)PartnerRequestStatusEnum.Sent, Name = "Invitation Sent",
                        Description = "Invitiation has been sent", DisplayOrder = 1, IsActive = true,
                    },
                    new PartnerRequestStatus
                    {
                        Id = (int)PartnerRequestStatusEnum.Approved, Name = "Approved",
                        Description = "Request has been approved by the Site Administrator", DisplayOrder = 2,
                        IsActive = true,
                    },
                    new PartnerRequestStatus
                    {
                        Id = (int)PartnerRequestStatusEnum.Denied, Name = "Denied",
                        Description = "Request has been approved by the Site Administrator", DisplayOrder = 3,
                        IsActive = true,
                    },
                    new PartnerRequestStatus
                    {
                        Id = (int)PartnerRequestStatusEnum.Pending, Name = "Pending Approval",
                        Description = "Invitiation is pending approval by TrshMob.eco admin", DisplayOrder = 4,
                        IsActive = true,
                    });
            });

            modelBuilder.Entity<PartnerStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new PartnerStatus
                    {
                        Id = (int)PartnerStatusEnum.Active, Name = "Active", Description = "Partner is Active",
                        DisplayOrder = 1, IsActive = true,
                    },
                    new PartnerStatus
                    {
                        Id = (int)PartnerStatusEnum.Inactive, Name = "Inactive", Description = "Partner is Inactive",
                        DisplayOrder = 2, IsActive = true,
                    });
            });

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new PartnerType
                    {
                        Id = (int)PartnerTypeEnum.Government, Name = "Government",
                        Description = "Partner is a Government or Government Agency", DisplayOrder = 1, IsActive = true,
                    },
                    new PartnerType
                    {
                        Id = (int)PartnerTypeEnum.Business, Name = "Business", Description = "Partner is Business",
                        DisplayOrder = 2, IsActive = true,
                    },
                    new PartnerType
                    {
                        Id = (int)PartnerTypeEnum.Community, Name = "Community",
                        Description = "Community organization with branded page and volunteer tools",
                        DisplayOrder = 3, IsActive = true,
                    });
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

                entity.Property(e => e.Name).HasMaxLength(50);

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
                    new ServiceType
                    {
                        Id = (int)ServiceTypeEnum.Hauling, Name = "Hauling",
                        Description = "Partner will haul litter away", DisplayOrder = 1, IsActive = true,
                    },
                    new ServiceType
                    {
                        Id = (int)ServiceTypeEnum.DisposalLocation, Name = "Disposal Location",
                        Description = "Partner will dispose of litter", DisplayOrder = 2, IsActive = true,
                    },
                    new ServiceType
                    {
                        Id = (int)ServiceTypeEnum.StartupKits, Name = "Startup Kits",
                        Description = "Partner distributes starter kits", DisplayOrder = 3, IsActive = true,
                    },
                    new ServiceType
                    {
                        Id = (int)ServiceTypeEnum.Supplies, Name = "Supplies",
                        Description = "Partner distributes supplies", DisplayOrder = 4, IsActive = true,
                    });
            });

            modelBuilder.Entity<SocialMediaAccountType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new SocialMediaAccountType
                    {
                        Id = (int)SocialMediaAccountTypeEnum.Facebook, Name = "Facebook", Description = "Facebook",
                        DisplayOrder = 1,
                    },
                    new SocialMediaAccountType
                    {
                        Id = (int)SocialMediaAccountTypeEnum.Twitter, Name = "Twitter", Description = "Twitter",
                        DisplayOrder = 2,
                    },
                    new SocialMediaAccountType
                    {
                        Id = (int)SocialMediaAccountTypeEnum.Instagram, Name = "Instagram", Description = "Instagram",
                        DisplayOrder = 3,
                    },
                    new SocialMediaAccountType
                    {
                        Id = (int)SocialMediaAccountTypeEnum.TikTok, Name = "TikTok", Description = "TikTok",
                        DisplayOrder = 4,
                    });
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

                entity.Property(e => e.GivenName).HasMaxLength(64);

                entity.Property(e => e.Surname).HasMaxLength(64);

                entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);

                entity.HasData(
                    new User
                    {
                        Id = Guid.Empty, City = "Anytown", Country = "AnyCountry", Email = "info@trashmob.eco",
                        Region = "AnyState", UserName = "TrashMob", ShowOnLeaderboards = false,
                    });

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
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.EventSummaryAttendee, Name = "EventSummaryAttendee",
                        Description = "Opt out of Post Event Summary", DisplayOrder = 1,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.EventSummaryHostReminder, Name = "EventSummaryHostReminder",
                        Description = "Opt out of Event Summary Reminder for events you have lead", DisplayOrder = 2,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UpcomingEventAttendingThisWeek,
                        Name = "UpcomingEventAttendingThisWeek",
                        Description = "Opt out of notifications for events upcoming this week you are attending",
                        DisplayOrder = 3,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UpcomingEventAttendingSoon, Name = "UpcomingEventAttendingSoon",
                        Description = "Opt out of notifications for events happening soon you are attending",
                        DisplayOrder = 4,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UpcomingEventHostingThisWeek,
                        Name = "UpcomingEventHostingThisWeek",
                        Description = "Opt out of notifications for events upcoming this week you are leading",
                        DisplayOrder = 5,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UpcomingEventHostingSoon, Name = "UpcomingEventHostingSoon",
                        Description = "Opt out of notifications for events happening soon you are leading",
                        DisplayOrder = 6,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek,
                        Name = "UpcomingEventsInYourAreaThisWeek",
                        Description = "Opt out of notification for new events upcoming in your area this week",
                        DisplayOrder = 7,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UpcomingEventsInYourAreaSoon,
                        Name = "UpcomingEventsInYourAreaSoon",
                        Description = "Opt out of notification for new events happening in your area soon",
                        DisplayOrder = 8,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.EventSummaryHostWeekReminder,
                        Name = "EventSummaryHostWeekReminder",
                        Description = "Opt out of Event Summary Week Reminder for events you have lead",
                        DisplayOrder = 9,
                    },
                    new UserNotificationType
                    {
                        Id = (int)NotificationTypeEnum.UserProfileUpdateLocation, Name = "UserProfileUpdateLocation",
                        Description = "Opt out of notifications for User Profile Location", DisplayOrder = 10,
                    });
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
                    new Waiver
                    {
                        Id = new Guid("4D222D04-AC1F-4A87-886D-FDB686F9F55C"), Name = "trashmob",
                        IsWaiverEnabled = false, CreatedByUserId = Guid.Empty, LastUpdatedByUserId = Guid.Empty,
                        CreatedDate = new DateTimeOffset(2022, 11, 24, 0, 0, 0, TimeSpan.Zero),
                    });
            });

            modelBuilder.Entity<LitterReportStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new LitterReportStatus
                    {
                        Id = (int)LitterReportStatusEnum.New, Name = "New", Description = "New created",
                        DisplayOrder = 1, IsActive = true,
                    },
                    new LitterReportStatus
                    {
                        Id = (int)LitterReportStatusEnum.Assigned, Name = "Assigned", Description = "Assigned To Event",
                        DisplayOrder = 2, IsActive = true,
                    },
                    new LitterReportStatus
                    {
                        Id = (int)LitterReportStatusEnum.Cleaned, Name = "Cleaned", Description = "Litter Cleaned",
                        DisplayOrder = 3, IsActive = true,
                    },
                    new LitterReportStatus
                    {
                        Id = (int)LitterReportStatusEnum.Cancelled, Name = "Cancelled",
                        Description = "Report Cancelled", DisplayOrder = 4, IsActive = true,
                    });
            });

            modelBuilder.Entity<LitterImage>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AzureBlobURL)
                    .IsRequired();

                entity.Property(e => e.IsCancelled).HasDefaultValue(false);

                entity.HasOne(d => d.LitterReport)
                    .WithMany(p => p.LitterImages)
                    .HasForeignKey(d => d.LitterReportId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_LitterImage_LitterReports");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.LitterImagesCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LitterImage_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.LitterImagesUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LitterImage_LastUpdatedBy");

                entity.Property(e => e.ModerationStatus).HasDefaultValue(PhotoModerationStatus.Pending);

                entity.Property(e => e.InReview).HasDefaultValue(false);

                entity.Property(e => e.ModerationReason).HasMaxLength(500);

                entity.HasOne(d => d.ReviewRequestedByUser)
                    .WithMany(p => p.LitterImagesReviewRequested)
                    .HasForeignKey(d => d.ReviewRequestedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_LitterImage_ReviewRequestedByUser");

                entity.HasOne(d => d.ModeratedByUser)
                    .WithMany(p => p.LitterImagesModerated)
                    .HasForeignKey(d => d.ModeratedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_LitterImage_ModeratedByUser");
            });

            modelBuilder.Entity<Models.LitterReport>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.CreatedByUserId);

                entity.Property(e => e.LastUpdatedByUserId);

                entity.HasOne(d => d.LitterReportStatus)
                    .WithMany(p => p.LitterReports)
                    .HasForeignKey(d => d.LitterReportStatusId)
                    .HasConstraintName("FK_LitterReport_LitterReportStatuses");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.LitterReportsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LitterReport_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.LitterReportsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LitterReport_LastUpdatedBy");
            });

            modelBuilder.Entity<EventLitterReport>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.LitterReportId });

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventLitterReports)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventLitterReport_Event");

                entity.HasOne(d => d.LitterReport)
                    .WithMany(p => p.EventLitterReports)
                    .HasForeignKey(d => d.LitterReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventLitterReport_LitterReport");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventLitterReportsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventLitterReport_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventLitterReportsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventLitterReport_LastUpdatedBy");
            });

            modelBuilder.Entity<WeightUnit>(entity =>
                {
                    entity.Property(e => e.Id).ValueGeneratedNever();

                    entity.Property(e => e.Description);

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.HasData(
                            new WeightUnit
                            {
                                Id = (int)WeightUnitEnum.None,
                                Name = "None",
                                Description = "Weight unit not set",
                                DisplayOrder = 1,
                                IsActive = true,
                            },
                            new WeightUnit
                            {
                                Id = (int)WeightUnitEnum.Pound,
                                Name = "lb",
                                Description = "Weight in Imperial Pounds",
                                DisplayOrder = 2,
                                IsActive = true,
                            },
                            new WeightUnit
                            {
                                Id = (int)WeightUnitEnum.Kilogram,
                                Name = "kg",
                                Description = "Weight in Kilograms",
                                DisplayOrder = 3,
                                IsActive = true,
                            });
                });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Teams_Name");

                entity.Property(e => e.Description)
                    .HasMaxLength(2048);

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasMaxLength(256);

                entity.Property(e => e.Region)
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .HasMaxLength(64);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(25);

                entity.Property(e => e.IsPublic)
                    .HasDefaultValue(true);

                entity.Property(e => e.RequiresApproval)
                    .HasDefaultValue(true);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.TeamsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teams_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.TeamsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teams_User_LastUpdatedBy");
            });

            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => new { e.TeamId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("IX_TeamMembers_TeamId_UserId");

                entity.HasIndex(e => new { e.TeamId, e.IsTeamLead })
                    .HasDatabaseName("IX_TeamMembers_TeamId_IsTeamLead");

                entity.HasOne(d => d.Team)
                    .WithMany(d => d.Members)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamMembers_Team");

                entity.HasOne(d => d.User)
                    .WithMany(d => d.TeamMemberships)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamMembers_User");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.TeamMembersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamMembers_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.TeamMembersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamMembers_User_LastUpdatedBy");
            });

            modelBuilder.Entity<TeamJoinRequest>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => new { e.TeamId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("IX_TeamJoinRequests_TeamId_UserId");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");

                entity.HasOne(d => d.Team)
                    .WithMany(d => d.JoinRequests)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamJoinRequests_Team");

                entity.HasOne(d => d.User)
                    .WithMany(d => d.TeamJoinRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamJoinRequests_User");

                entity.HasOne(d => d.ReviewedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamJoinRequests_ReviewedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.TeamJoinRequestsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamJoinRequests_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.TeamJoinRequestsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamJoinRequests_User_LastUpdatedBy");
            });

            modelBuilder.Entity<TeamAdoption>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => new { e.TeamId, e.AdoptableAreaId })
                    .IsUnique()
                    .HasDatabaseName("IX_TeamAdoptions_TeamId_AdoptableAreaId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_TeamAdoptions_Status");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.ApplicationNotes)
                    .HasMaxLength(2048);

                entity.Property(e => e.RejectionReason)
                    .HasMaxLength(1000);

                // Compliance tracking fields
                entity.Property(e => e.EventCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsCompliant)
                    .HasDefaultValue(true);

                entity.HasOne(d => d.Team)
                    .WithMany(d => d.Adoptions)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamAdoptions_Team");

                entity.HasOne(d => d.AdoptableArea)
                    .WithMany(d => d.Adoptions)
                    .HasForeignKey(d => d.AdoptableAreaId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TeamAdoptions_AdoptableArea");

                entity.HasOne(d => d.ReviewedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamAdoptions_ReviewedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamAdoptions_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamAdoptions_User_LastUpdatedBy");
            });

            modelBuilder.Entity<TeamAdoptionEvent>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => new { e.TeamAdoptionId, e.EventId })
                    .IsUnique()
                    .HasDatabaseName("IX_TeamAdoptionEvents_TeamAdoptionId_EventId");

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                entity.HasOne(d => d.TeamAdoption)
                    .WithMany(d => d.AdoptionEvents)
                    .HasForeignKey(d => d.TeamAdoptionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamAdoptionEvents_TeamAdoption");

                entity.HasOne(d => d.Event)
                    .WithMany(d => d.AdoptionEvents)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TeamAdoptionEvents_Event");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamAdoptionEvents_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamAdoptionEvents_User_LastUpdatedBy");
            });

            // Sponsored Adoptions

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(256);

                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(30);

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(2048);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.ShowOnPublicMap)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.PartnerId)
                    .HasDatabaseName("IX_Sponsors_PartnerId");

                entity.HasOne(e => e.Partner)
                    .WithMany(p => p.Sponsors)
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Sponsors_Partners");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sponsors_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sponsors_User_LastUpdatedBy");
            });

            modelBuilder.Entity<ProfessionalCompany>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(256);

                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(30);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.PartnerId)
                    .HasDatabaseName("IX_ProfessionalCompanies_PartnerId");

                entity.HasOne(e => e.Partner)
                    .WithMany(p => p.ProfessionalCompanies)
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProfessionalCompanies_Partners");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCompanies_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCompanies_User_LastUpdatedBy");
            });

            modelBuilder.Entity<ProfessionalCompanyUser>(entity =>
            {
                entity.HasKey(e => new { e.ProfessionalCompanyId, e.UserId });

                entity.HasOne(d => d.ProfessionalCompany)
                    .WithMany(d => d.CompanyUsers)
                    .HasForeignKey(d => d.ProfessionalCompanyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProfessionalCompanyUsers_ProfessionalCompany");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCompanyUsers_User");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCompanyUsers_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCompanyUsers_User_LastUpdatedBy");
            });

            modelBuilder.Entity<SponsoredAdoption>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Active");

                entity.Property(e => e.CleanupFrequencyDays)
                    .HasDefaultValue(14);

                entity.HasIndex(e => e.AdoptableAreaId)
                    .HasDatabaseName("IX_SponsoredAdoptions_AdoptableAreaId");

                entity.HasIndex(e => e.SponsorId)
                    .HasDatabaseName("IX_SponsoredAdoptions_SponsorId");

                entity.HasIndex(e => e.ProfessionalCompanyId)
                    .HasDatabaseName("IX_SponsoredAdoptions_ProfessionalCompanyId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_SponsoredAdoptions_Status");

                entity.HasOne(d => d.AdoptableArea)
                    .WithMany(d => d.SponsoredAdoptions)
                    .HasForeignKey(d => d.AdoptableAreaId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_SponsoredAdoptions_AdoptableArea");

                entity.HasOne(d => d.Sponsor)
                    .WithMany(d => d.SponsoredAdoptions)
                    .HasForeignKey(d => d.SponsorId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_SponsoredAdoptions_Sponsor");

                entity.HasOne(d => d.ProfessionalCompany)
                    .WithMany(d => d.SponsoredAdoptions)
                    .HasForeignKey(d => d.ProfessionalCompanyId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_SponsoredAdoptions_ProfessionalCompany");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SponsoredAdoptions_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SponsoredAdoptions_User_LastUpdatedBy");
            });

            modelBuilder.Entity<ProfessionalCleanupLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CleanupDate).IsRequired();

                entity.Property(e => e.WeightInPounds)
                    .HasPrecision(10, 2);

                entity.Property(e => e.WeightInKilograms)
                    .HasPrecision(10, 2);

                entity.Property(e => e.Notes)
                    .HasMaxLength(4000);

                entity.HasIndex(e => e.SponsoredAdoptionId)
                    .HasDatabaseName("IX_ProfessionalCleanupLogs_SponsoredAdoptionId");

                entity.HasIndex(e => e.ProfessionalCompanyId)
                    .HasDatabaseName("IX_ProfessionalCleanupLogs_ProfessionalCompanyId");

                entity.HasIndex(e => e.CleanupDate)
                    .HasDatabaseName("IX_ProfessionalCleanupLogs_CleanupDate");

                entity.HasOne(d => d.SponsoredAdoption)
                    .WithMany(d => d.CleanupLogs)
                    .HasForeignKey(d => d.SponsoredAdoptionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProfessionalCleanupLogs_SponsoredAdoption");

                entity.HasOne(d => d.ProfessionalCompany)
                    .WithMany(d => d.CleanupLogs)
                    .HasForeignKey(d => d.ProfessionalCompanyId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_ProfessionalCleanupLogs_ProfessionalCompany");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCleanupLogs_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionalCleanupLogs_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EventAttendeeMetrics>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                // Unique constraint: one submission per attendee per event
                entity.HasIndex(e => new { e.EventId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("IX_EventAttendeeMetrics_EventId_UserId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_EventAttendeeMetrics_Status");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.PickedWeight)
                    .HasColumnType("decimal(10,1)");

                entity.Property(e => e.AdjustedPickedWeight)
                    .HasColumnType("decimal(10,1)");

                entity.Property(e => e.Notes)
                    .HasMaxLength(2048);

                entity.Property(e => e.RejectionReason)
                    .HasMaxLength(1000);

                entity.Property(e => e.AdjustmentReason)
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.AttendeeMetrics)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EventAttendeeMetrics_Event");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AttendeeMetrics)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EventAttendeeMetrics_User");

                entity.HasOne(d => d.ReviewedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeMetrics_ReviewedByUser");

                entity.HasOne(d => d.PickedWeightUnit)
                    .WithMany(p => p.AttendeeMetricsForPicked)
                    .HasForeignKey(d => d.PickedWeightUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeMetrics_PickedWeightUnit");

                entity.HasOne(d => d.AdjustedPickedWeightUnit)
                    .WithMany(p => p.AttendeeMetricsForAdjusted)
                    .HasForeignKey(d => d.AdjustedPickedWeightUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeMetrics_AdjustedPickedWeightUnit");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeMetrics_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAttendeeMetrics_User_LastUpdatedBy");
            });

            modelBuilder.Entity<TeamEvent>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(e => new { e.TeamId, e.EventId })
                    .IsUnique()
                    .HasDatabaseName("IX_TeamEvents_TeamId_EventId");

                entity.HasOne(d => d.Team)
                    .WithMany(d => d.TeamEvents)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamEvents_Team");

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamEvents_Event");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.TeamEventsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamEvents_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.TeamEventsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamEvents_User_LastUpdatedBy");
            });

            modelBuilder.Entity<TeamPhoto>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Caption)
                    .HasMaxLength(500);

                entity.HasOne(d => d.Team)
                    .WithMany(d => d.Photos)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TeamPhotos_Team");

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.UploadedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamPhotos_UploadedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.TeamPhotosCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamPhotos_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.TeamPhotosUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamPhotos_User_LastUpdatedBy");

                entity.Property(e => e.ModerationStatus).HasDefaultValue(PhotoModerationStatus.Pending);

                entity.Property(e => e.InReview).HasDefaultValue(false);

                entity.Property(e => e.ModerationReason).HasMaxLength(500);

                entity.HasOne(d => d.ReviewRequestedByUser)
                    .WithMany(p => p.TeamPhotosReviewRequested)
                    .HasForeignKey(d => d.ReviewRequestedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TeamPhotos_ReviewRequestedByUser");

                entity.HasOne(d => d.ModeratedByUser)
                    .WithMany(p => p.TeamPhotosModerated)
                    .HasForeignKey(d => d.ModeratedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_TeamPhotos_ModeratedByUser");
            });

            modelBuilder.Entity<PartnerPhoto>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Caption)
                    .HasMaxLength(500);

                entity.HasOne(d => d.Partner)
                    .WithMany(d => d.Photos)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PartnerPhotos_Partner");

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(p => p.PartnerPhotosUploaded)
                    .HasForeignKey(d => d.UploadedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerPhotos_UploadedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PartnerPhotosCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerPhotos_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.PartnerPhotosUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerPhotos_User_LastUpdatedBy");

                entity.Property(e => e.ModerationStatus).HasDefaultValue(PhotoModerationStatus.Pending);

                entity.Property(e => e.InReview).HasDefaultValue(false);

                entity.Property(e => e.ModerationReason).HasMaxLength(500);

                entity.HasOne(d => d.ReviewRequestedByUser)
                    .WithMany(p => p.PartnerPhotosReviewRequested)
                    .HasForeignKey(d => d.ReviewRequestedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_PartnerPhotos_ReviewRequestedByUser");

                entity.HasOne(d => d.ModeratedByUser)
                    .WithMany(p => p.PartnerPhotosModerated)
                    .HasForeignKey(d => d.ModeratedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_PartnerPhotos_ModeratedByUser");
            });

            modelBuilder.Entity<EventPhoto>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ThumbnailUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.Caption)
                    .HasMaxLength(500);

                entity.HasOne(d => d.Event)
                    .WithMany(d => d.EventPhotos)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EventPhotos_Event");

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(p => p.EventPhotosUploaded)
                    .HasForeignKey(d => d.UploadedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPhotos_UploadedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.EventPhotosCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPhotos_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.EventPhotosUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventPhotos_User_LastUpdatedBy");

                entity.Property(e => e.ModerationStatus).HasDefaultValue(PhotoModerationStatus.Pending);

                entity.Property(e => e.InReview).HasDefaultValue(false);

                entity.Property(e => e.ModerationReason).HasMaxLength(500);

                entity.HasOne(d => d.ReviewRequestedByUser)
                    .WithMany(p => p.EventPhotosReviewRequested)
                    .HasForeignKey(d => d.ReviewRequestedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EventPhotos_ReviewRequestedByUser");

                entity.HasOne(d => d.ModeratedByUser)
                    .WithMany(p => p.EventPhotosModerated)
                    .HasForeignKey(d => d.ModeratedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EventPhotos_ModeratedByUser");

                entity.HasIndex(e => e.EventId);
                entity.HasIndex(e => e.ModerationStatus);
                entity.HasIndex(e => e.PhotoType);
            });

            modelBuilder.Entity<UserFeedback>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Email)
                    .HasMaxLength(256);

                entity.Property(e => e.ScreenshotUrl)
                    .HasMaxLength(2048);

                entity.Property(e => e.PageUrl)
                    .HasMaxLength(2048);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(1024);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("New");

                entity.Property(e => e.InternalNotes)
                    .HasMaxLength(4000);

                entity.Property(e => e.GitHubIssueUrl)
                    .HasMaxLength(2048);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFeedbackSubmitted)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserFeedback_User");

                entity.HasOne(d => d.ReviewedByUser)
                    .WithMany(p => p.UserFeedbackReviewed)
                    .HasForeignKey(d => d.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserFeedback_ReviewedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.UserFeedbackCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeedback_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.UserFeedbackUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeedback_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PhotoFlag>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PhotoType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FlagReason)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Resolution)
                    .HasMaxLength(50);

                entity.HasOne(d => d.FlaggedByUser)
                    .WithMany(p => p.PhotoFlagsFlagged)
                    .HasForeignKey(d => d.FlaggedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_PhotoFlag_FlaggedByUser");

                entity.HasOne(d => d.ResolvedByUser)
                    .WithMany(p => p.PhotoFlagsResolved)
                    .HasForeignKey(d => d.ResolvedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_PhotoFlag_ResolvedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoFlag_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoFlag_User_LastUpdatedBy");
            });

            modelBuilder.Entity<PhotoModerationLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PhotoType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Reason)
                    .HasMaxLength(500);

                entity.HasOne(d => d.PerformedByUser)
                    .WithMany(p => p.PhotoModerationLogsPerformed)
                    .HasForeignKey(d => d.PerformedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_PhotoModerationLog_PerformedByUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoModerationLog_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhotoModerationLog_User_LastUpdatedBy");
            });

            // Waiver V3 Entities

            modelBuilder.Entity<WaiverVersion>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.WaiverText)
                    .IsRequired();

                entity.Property(e => e.Scope)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(e => new { e.Name, e.Version })
                    .IsUnique()
                    .HasDatabaseName("IX_WaiverVersions_Name_Version");

                entity.HasIndex(e => new { e.Scope, e.IsActive })
                    .HasDatabaseName("IX_WaiverVersions_Scope_IsActive");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.WaiverVersionsCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WaiverVersions_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.WaiverVersionsUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WaiverVersions_User_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityWaiver>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsRequired)
                    .HasDefaultValue(true);

                entity.HasIndex(e => new { e.CommunityId, e.WaiverVersionId })
                    .IsUnique()
                    .HasDatabaseName("IX_CommunityWaivers_CommunityId_WaiverVersionId");

                entity.HasOne(d => d.Community)
                    .WithMany(p => p.CommunityWaivers)
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CommunityWaivers_Partner");

                entity.HasOne(d => d.WaiverVersion)
                    .WithMany(p => p.CommunityWaivers)
                    .HasForeignKey(d => d.WaiverVersionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CommunityWaivers_WaiverVersion");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CommunityWaiversCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityWaivers_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.CommunityWaiversUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityWaivers_User_LastUpdatedBy");
            });

            modelBuilder.Entity<UserWaiver>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.TypedLegalName)
                    .HasMaxLength(200);

                entity.Property(e => e.SigningMethod)
                    .HasMaxLength(50);

                entity.Property(e => e.DocumentUrl)
                    .HasMaxLength(2048);

                entity.Property(e => e.IPAddress)
                    .HasMaxLength(50);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(500);

                entity.Property(e => e.GuardianName)
                    .HasMaxLength(200);

                entity.Property(e => e.GuardianRelationship)
                    .HasMaxLength(100);

                entity.Property(e => e.IsMinor)
                    .HasDefaultValue(false);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_UserWaivers_UserId");

                entity.HasIndex(e => e.WaiverVersionId)
                    .HasDatabaseName("IX_UserWaivers_WaiverVersionId");

                entity.HasIndex(e => e.ExpiryDate)
                    .HasDatabaseName("IX_UserWaivers_ExpiryDate");

                entity.HasIndex(e => new { e.UserId, e.WaiverVersionId })
                    .HasDatabaseName("IX_UserWaivers_UserId_WaiverVersionId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserWaivers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserWaivers_User");

                entity.HasOne(d => d.WaiverVersion)
                    .WithMany(p => p.UserWaivers)
                    .HasForeignKey(d => d.WaiverVersionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserWaivers_WaiverVersion");

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(p => p.UserWaiversUploaded)
                    .HasForeignKey(d => d.UploadedByUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserWaivers_UploadedByUser");

                entity.HasOne(d => d.GuardianUser)
                    .WithMany(p => p.UserWaiversAsGuardian)
                    .HasForeignKey(d => d.GuardianUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_UserWaivers_GuardianUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.UserWaiversCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserWaivers_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.UserWaiversUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserWaivers_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EmailInviteBatch>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BatchType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");

                entity.HasIndex(e => e.SenderUserId)
                    .HasDatabaseName("IX_EmailInviteBatches_SenderUserId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_EmailInviteBatches_Status");

                entity.HasIndex(e => e.CreatedDate)
                    .HasDatabaseName("IX_EmailInviteBatches_CreatedDate");

                entity.HasOne(d => d.SenderUser)
                    .WithMany()
                    .HasForeignKey(d => d.SenderUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EmailInviteBatches_SenderUser");

                entity.HasOne(d => d.Community)
                    .WithMany()
                    .HasForeignKey(d => d.CommunityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EmailInviteBatches_Community");

                entity.HasOne(d => d.Team)
                    .WithMany()
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EmailInviteBatches_Team");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailInviteBatches_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailInviteBatches_User_LastUpdatedBy");
            });

            modelBuilder.Entity<EmailInvite>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(1000);

                entity.HasIndex(e => e.BatchId)
                    .HasDatabaseName("IX_EmailInvites_BatchId");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_EmailInvites_Email");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_EmailInvites_Status");

                entity.HasOne(d => d.Batch)
                    .WithMany(b => b.Invites)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EmailInvites_Batch");

                entity.HasOne(d => d.SignedUpUser)
                    .WithMany()
                    .HasForeignKey(d => d.SignedUpUserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_EmailInvites_SignedUpUser");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailInvites_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailInvites_User_LastUpdatedBy");
            });

            // Gamification Entities

            modelBuilder.Entity<LeaderboardCache>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EntityType)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.EntityName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LeaderboardType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TimeRange)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LocationScope)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LocationValue)
                    .HasMaxLength(256);

                entity.Property(e => e.Score)
                    .HasColumnType("decimal(18,2)");

                // Index for efficient leaderboard queries
                entity.HasIndex(e => new { e.EntityType, e.LeaderboardType, e.TimeRange, e.LocationScope, e.LocationValue, e.Rank })
                    .HasDatabaseName("IX_LeaderboardCache_Lookup");

                // Index for entity-specific lookups
                entity.HasIndex(e => e.EntityId)
                    .HasDatabaseName("IX_LeaderboardCache_EntityId");
            });

            // Newsletter Entities

            modelBuilder.Entity<NewsletterCategory>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<NewsletterTemplate>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.ThumbnailUrl)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Newsletter>(entity =>
            {
                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PreviewText)
                    .HasMaxLength(500);

                entity.Property(e => e.TargetType)
                    .HasMaxLength(50)
                    .HasDefaultValue("All");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Draft");

                entity.HasOne(d => d.Category)
                    .WithMany(c => c.Newsletters)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Newsletters_NewsletterCategory");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.NewslettersCreated)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Newsletters_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany(p => p.NewslettersUpdated)
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Newsletters_User_LastUpdatedBy");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Newsletters_Status");

                entity.HasIndex(e => e.ScheduledDate)
                    .HasFilter("[Status] = 'Scheduled'")
                    .HasDatabaseName("IX_Newsletters_Scheduled");
            });

            modelBuilder.Entity<UserNewsletterPreference>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CategoryId });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.NewsletterPreferences)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserNewsletterPreferences_User");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.UserPreferences)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserNewsletterPreferences_Category");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_UserNewsletterPreferences_UserId");
            });

            // Achievement Entities

            modelBuilder.Entity<AchievementType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IconUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.Criteria)
                    .IsRequired()
                    .HasMaxLength(1000);

                // Seed initial achievement types
                entity.HasData(
                    new AchievementType
                    {
                        Id = 1, Name = "FirstSteps", DisplayName = "First Steps",
                        Description = "Attended your first cleanup event",
                        Category = "Participation", Criteria = "{\"eventsAttended\": 1}",
                        Points = 10, DisplayOrder = 1, IsActive = true,
                    },
                    new AchievementType
                    {
                        Id = 2, Name = "RegularVolunteer", DisplayName = "Regular Volunteer",
                        Description = "Attended 10 cleanup events",
                        Category = "Participation", Criteria = "{\"eventsAttended\": 10}",
                        Points = 50, DisplayOrder = 2, IsActive = true,
                    },
                    new AchievementType
                    {
                        Id = 3, Name = "DedicatedVolunteer", DisplayName = "Dedicated Volunteer",
                        Description = "Attended 25 cleanup events",
                        Category = "Participation", Criteria = "{\"eventsAttended\": 25}",
                        Points = 100, DisplayOrder = 3, IsActive = true,
                    },
                    new AchievementType
                    {
                        Id = 4, Name = "TrashCollector", DisplayName = "Trash Collector",
                        Description = "Collected 10 bags of trash",
                        Category = "Impact", Criteria = "{\"bagsCollected\": 10}",
                        Points = 25, DisplayOrder = 4, IsActive = true,
                    },
                    new AchievementType
                    {
                        Id = 5, Name = "TrashHero", DisplayName = "Trash Hero",
                        Description = "Collected 100 bags of trash",
                        Category = "Impact", Criteria = "{\"bagsCollected\": 100}",
                        Points = 150, DisplayOrder = 5, IsActive = true,
                    },
                    new AchievementType
                    {
                        Id = 6, Name = "TeamPlayer", DisplayName = "Team Player",
                        Description = "Joined a cleanup team",
                        Category = "Special", Criteria = "{\"joinedTeam\": true}",
                        Points = 20, DisplayOrder = 6, IsActive = true,
                    },
                    new AchievementType
                    {
                        Id = 7, Name = "EventCreator", DisplayName = "Event Creator",
                        Description = "Created your first cleanup event",
                        Category = "Special", Criteria = "{\"eventsCreated\": 1}",
                        Points = 30, DisplayOrder = 7, IsActive = true,
                    });
            });

            modelBuilder.Entity<UserAchievement>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                // Unique constraint: each user can only earn each achievement once
                entity.HasIndex(e => new { e.UserId, e.AchievementTypeId })
                    .IsUnique()
                    .HasDatabaseName("IX_UserAchievements_UserId_AchievementTypeId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Achievements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserAchievements_User");

                entity.HasOne(d => d.AchievementType)
                    .WithMany(p => p.UserAchievements)
                    .HasForeignKey(d => d.AchievementTypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserAchievements_AchievementType");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAchievements_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAchievements_User_LastUpdatedBy");
            });

            modelBuilder.Entity<CommunityProspect>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Type)
                    .HasMaxLength(50);

                entity.Property(e => e.City)
                    .HasMaxLength(256);

                entity.Property(e => e.Region)
                    .HasMaxLength(256);

                entity.Property(e => e.Country)
                    .HasMaxLength(64);

                entity.Property(e => e.Website)
                    .HasMaxLength(2048);

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(256);

                entity.Property(e => e.ContactName)
                    .HasMaxLength(128);

                entity.Property(e => e.ContactTitle)
                    .HasMaxLength(128);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.PipelineStage)
                    .HasDefaultValue(0);

                entity.Property(e => e.FitScore)
                    .HasDefaultValue(0);

                entity.HasOne<Partner>()
                    .WithMany()
                    .HasForeignKey(d => d.ConvertedPartnerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CommunityProspects_Partner");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityProspects_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommunityProspects_User_LastUpdatedBy");
            });

            modelBuilder.Entity<ProspectActivity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ActivityType)
                    .HasMaxLength(50);

                entity.Property(e => e.Subject)
                    .HasMaxLength(256);

                entity.Property(e => e.Details)
                    .HasMaxLength(4000);

                entity.Property(e => e.SentimentScore)
                    .HasMaxLength(20);

                entity.HasOne(d => d.Prospect)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.ProspectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProspectActivities_CommunityProspect");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProspectActivities_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProspectActivities_User_LastUpdatedBy");
            });

            modelBuilder.Entity<ProspectOutreachEmail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Subject)
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(2000);

                entity.HasIndex(e => e.ProspectId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(d => d.Prospect)
                    .WithMany(p => p.OutreachEmails)
                    .HasForeignKey(d => d.ProspectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProspectOutreachEmails_CommunityProspect");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProspectOutreachEmails_User_CreatedBy");

                entity.HasOne(d => d.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.LastUpdatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProspectOutreachEmails_User_LastUpdatedBy");
            });
        }
    }
}