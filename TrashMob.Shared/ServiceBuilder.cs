namespace TrashMob.Shared
{
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Events;
    using TrashMob.Shared.Managers.IFTTT;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.LitterReport;
    using TrashMob.Shared.Managers.Partners;
    using TrashMob.Shared.Managers.Teams;
    using TrashMob.Shared.Managers.Communities;
    using TrashMob.Shared.Managers.Adoptions;
    using TrashMob.Shared.Managers.Areas;
    using TrashMob.Shared.Managers.Gamification;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Managers.SponsoredAdoptions;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Services;

    public static class ServiceBuilder
    {
        public static IServiceCollection AddManagers(this IServiceCollection services)
        {
            // Migrated Managers
            services.AddScoped<IKeyedManager<ContactRequest>, ContactRequestManager>();
            services.AddScoped<IBaseManager<EventAttendee>, EventAttendeeManager>();
            services.AddScoped<IKeyedManager<EventAttendeeRoute>, EventAttendeeRouteManager>();
            services.AddScoped<IBaseManager<EventLitterReport>, EventLitterReportManager>();
            services.AddScoped<IKeyedManager<Event>, EventManager>();
            services
                .AddScoped<ILookupManager<EventPartnerLocationServiceStatus>,
                    LookupManager<EventPartnerLocationServiceStatus>>();
            services.AddScoped<IBaseManager<EventPartnerLocationService>, EventPartnerLocationServiceManager>();
            services.AddScoped<ILookupManager<EventStatus>, LookupManager<EventStatus>>();
            services.AddScoped<IBaseManager<EventSummary>, EventSummaryManager>();
            services.AddScoped<ILookupManager<EventType>, LookupManager<EventType>>();
            services.AddScoped<ILookupManager<InvitationStatus>, LookupManager<InvitationStatus>>();
            services.AddScoped<IKeyedManager<JobOpportunity>, KeyedManager<JobOpportunity>>();
            services.AddScoped<IKeyedManager<Partner>, KeyedManager<Partner>>();
            services.AddScoped<IKeyedManager<PartnerDocument>, PartnerDocumentManager>();
            services.AddScoped<IKeyedManager<PartnerContact>, KeyedManager<PartnerContact>>();
            services.AddScoped<IKeyedManager<PartnerLocation>, KeyedManager<PartnerLocation>>();
            services.AddScoped<IKeyedManager<PartnerLocationContact>, KeyedManager<PartnerLocationContact>>();
            services.AddScoped<IBaseManager<PartnerLocationService>, PartnerLocationServiceManager>();
            services.AddScoped<IKeyedManager<PartnerRequest>, PartnerRequestManager>();
            services.AddScoped<ILookupManager<PartnerRequestStatus>, LookupManager<PartnerRequestStatus>>();
            services.AddScoped<IKeyedManager<PartnerSocialMediaAccount>, PartnerSocialMediaAccountManager>();
            services.AddScoped<ILookupManager<PartnerStatus>, LookupManager<PartnerStatus>>();
            services.AddScoped<ILookupManager<PartnerType>, LookupManager<PartnerType>>();
            services.AddScoped<IBaseManager<PartnerAdmin>, PartnerAdminManager>();
            services.AddScoped<IKeyedManager<PickupLocation>, PickupLocationManager>();
            services.AddScoped<ILookupManager<ServiceType>, LookupManager<ServiceType>>();
            services.AddScoped<ILookupManager<SocialMediaAccountType>, LookupManager<SocialMediaAccountType>>();
            services.AddScoped<IKeyedManager<NonEventUserNotification>, KeyedManager<NonEventUserNotification>>();
            services.AddScoped<IUserDeletionService, UserDeletionService>();
            services.AddScoped<IUserDataExportManager, UserDataExportManager>();
            services.AddScoped<IKeyedManager<User>, UserManager>();
            services.AddScoped<IKeyedManager<UserNotification>, UserNotificationManager>();
            services.AddScoped<ILookupManager<WeightUnit>, LookupManager<WeightUnit>>();
            services.AddScoped<IKeyedManager<LitterReport>, LitterReportManager>();
            services.AddScoped<IKeyedManager<LitterImage>, LitterImageManager>();
            services.AddScoped<IKeyedManager<EventPhoto>, EventPhotoManager>();

            // Intentional deviation due to unique methods
            services.AddScoped<IEventAttendeeManager, EventAttendeeManager>();
            services.AddScoped<IEventAttendeeRouteManager, EventAttendeeRouteManager>();
            services.AddScoped<IEventLitterReportManager, EventLitterReportManager>();
            services.AddScoped<IEventSummaryManager, EventSummaryManager>();
            services.AddScoped<IEventAttendeeMetricsManager, EventAttendeeMetricsManager>();
            services.AddScoped<IEventManager, EventManager>();
            services.AddScoped<IEventPartnerLocationServiceManager, EventPartnerLocationServiceManager>();
            services.AddScoped<IPartnerAdminManager, PartnerAdminManager>();
            services.AddScoped<IPartnerAdminInvitationManager, PartnerAdminInvitationManager>();
            services.AddScoped<IPartnerContactManager, PartnerContactManager>();
            services.AddScoped<IPartnerDocumentManager, PartnerDocumentManager>();
            services.AddScoped<IPartnerRequestManager, PartnerRequestManager>();
            services.AddScoped<IPartnerLocationManager, PartnerLocationManager>();
            services.AddScoped<IPartnerLocationContactManager, PartnerLocationContactManager>();
            services.AddScoped<IPartnerSocialMediaAccountManager, PartnerSocialMediaAccountManager>();
            services.AddScoped<IPickupLocationManager, PickupLocationManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<INonEventUserNotificationManager, NonEventUserNotificationManager>();
            services.AddScoped<IWaiverManager, WaiverManager>();
            services.AddScoped<ILitterImageManager, LitterImageManager>();
            services.AddScoped<ILitterReportManager, LitterReportManager>();

            // Team managers
            services.AddScoped<ITeamManager, TeamManager>();
            services.AddScoped<ITeamMemberManager, TeamMemberManager>();
            services.AddScoped<ITeamPhotoManager, TeamPhotoManager>();

            // Event Photo managers
            services.AddScoped<IEventPhotoManager, EventPhotoManager>();

            // Partner Photo managers
            services.AddScoped<IPartnerPhotoManager, PartnerPhotoManager>();

            // Partner Document Storage manager
            services.AddScoped<IPartnerDocumentStorageManager, PartnerDocumentStorageManager>();

            // Community managers
            services.AddScoped<ICommunityManager, CommunityManager>();

            // Adoption managers
            services.AddScoped<IAdoptableAreaManager, AdoptableAreaManager>();
            services.AddScoped<IAreaSuggestionService, AreaSuggestionService>();
            services.AddHttpClient<INominatimService, NominatimService>();
            services.AddScoped<IAreaFileParser, AreaFileParser>();
            services.AddScoped<ITeamAdoptionManager, TeamAdoptionManager>();
            services.AddScoped<ITeamAdoptionEventManager, TeamAdoptionEventManager>();

            // Area generation managers
            services.AddScoped<IAreaGenerationBatchManager, AreaGenerationBatchManager>();
            services.AddScoped<IStagedAdoptableAreaManager, StagedAdoptableAreaManager>();
            services.AddScoped<IAreaGenerationOrchestrator, AreaGenerationOrchestrator>();

            // Sponsored adoption managers
            services.AddScoped<ISponsorManager, SponsorManager>();
            services.AddScoped<IProfessionalCompanyManager, ProfessionalCompanyManager>();
            services.AddScoped<IProfessionalCompanyUserManager, ProfessionalCompanyUserManager>();
            services.AddScoped<ISponsoredAdoptionManager, SponsoredAdoptionManager>();
            services.AddScoped<IProfessionalCleanupLogManager, ProfessionalCleanupLogManager>();

            // User Feedback
            services.AddScoped<IUserFeedbackManager, UserFeedbackManager>();

            // Photo Moderation
            services.AddScoped<IPhotoModerationManager, PhotoModerationManager>();

            // Waiver V3
            services.AddScoped<IWaiverVersionManager, WaiverVersionManager>();
            services.AddScoped<IUserWaiverManager, UserWaiverManager>();
            services.AddScoped<IWaiverDocumentManager, WaiverDocumentManager>();

            // Feature Metrics
            services.AddSingleton<IFeatureMetricsService, FeatureMetricsService>();

            // Email Invites
            services.AddScoped<IEmailInviteManager, EmailInviteManager>();

            // Gamification
            services.AddScoped<ILeaderboardManager, LeaderboardManager>();
            services.AddScoped<IAchievementManager, AchievementManager>();

            // Community Prospects
            services.AddScoped<ICommunityProspectManager, CommunityProspectManager>();
            services.AddScoped<IProspectActivityManager, ProspectActivityManager>();
            services.AddScoped<IClaudeDiscoveryService, ClaudeDiscoveryService>();
            services.AddScoped<IProspectScoringManager, ProspectScoringManager>();
            services.AddScoped<ICsvImportManager, CsvImportManager>();
            services.AddScoped<IOutreachContentService, OutreachContentService>();
            services.AddScoped<IProspectOutreachManager, ProspectOutreachManager>();
            services.AddScoped<IPipelineAnalyticsManager, PipelineAnalyticsManager>();
            services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
            services.AddScoped<IProspectConversionManager, ProspectConversionManager>();

            // Newsletter
            services.AddScoped<INewsletterManager, NewsletterManager>();
            services.AddScoped<IUserNewsletterPreferenceManager, UserNewsletterPreferenceManager>();

            // Non-patterned
            services.AddScoped<IActiveDirectoryManager, ActiveDirectoryManager>();
            services.AddScoped<IEmailManager, EmailManager>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IImageManager, ImageManager>();
            services.AddScoped<IMapManager, MapManager>();
            services.AddScoped<ISecretRepository, SecretRepository>();
            services.AddScoped<IQueriesManager, QueriesManager>();
            services.AddScoped<ITriggersManager, TriggersManager>();
            services.AddScoped<IDbTransaction, DbTransaction>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Migrated Repositories
            services.AddScoped<IKeyedRepository<ContactRequest>, KeyedRepository<ContactRequest>>();
            services.AddScoped<IKeyedRepository<Event>, KeyedRepository<Event>>();
            services.AddScoped<IBaseRepository<EventAttendee>, BaseRepository<EventAttendee>>();
            services.AddScoped<IKeyedRepository<EventAttendeeRoute>, KeyedRepository<EventAttendeeRoute>>();
            services.AddScoped<IBaseRepository<EventLitterReport>, BaseRepository<EventLitterReport>>();
            services
                .AddScoped<IBaseRepository<EventPartnerLocationService>, BaseRepository<EventPartnerLocationService>>();
            services
                .AddScoped<ILookupRepository<EventPartnerLocationServiceStatus>,
                    LookupRepository<EventPartnerLocationServiceStatus>>();
            services.AddScoped<ILookupRepository<EventStatus>, LookupRepository<EventStatus>>();
            services.AddScoped<IBaseRepository<EventSummary>, BaseRepository<EventSummary>>();
            services.AddScoped<IKeyedRepository<EventAttendeeMetrics>, KeyedRepository<EventAttendeeMetrics>>();
            services.AddScoped<ILookupRepository<EventType>, LookupRepository<EventType>>();
            services.AddScoped<IBaseRepository<IftttTrigger>, BaseRepository<IftttTrigger>>();
            services.AddScoped<ILookupRepository<InvitationStatus>, LookupRepository<InvitationStatus>>();
            services.AddScoped<IKeyedRepository<JobOpportunity>, KeyedRepository<JobOpportunity>>();
            services.AddScoped<IKeyedRepository<LitterImage>, KeyedRepository<LitterImage>>();
            services.AddScoped<IKeyedRepository<LitterReport>, KeyedRepository<LitterReport>>();
            services.AddScoped<IKeyedRepository<MessageRequest>, KeyedRepository<MessageRequest>>();
            services.AddScoped<IKeyedRepository<NonEventUserNotification>, KeyedRepository<NonEventUserNotification>>();
            services.AddScoped<IKeyedRepository<Partner>, KeyedRepository<Partner>>();
            services.AddScoped<IBaseRepository<PartnerAdmin>, BaseRepository<PartnerAdmin>>();
            services.AddScoped<IKeyedRepository<PartnerAdminInvitation>, KeyedRepository<PartnerAdminInvitation>>();
            services.AddScoped<IKeyedRepository<PartnerContact>, KeyedRepository<PartnerContact>>();
            services.AddScoped<IKeyedRepository<PartnerDocument>, KeyedRepository<PartnerDocument>>();
            services.AddScoped<IKeyedRepository<PartnerLocation>, KeyedRepository<PartnerLocation>>();
            services.AddScoped<IKeyedRepository<PartnerLocationContact>, KeyedRepository<PartnerLocationContact>>();
            services.AddScoped<IBaseRepository<PartnerLocationService>, BaseRepository<PartnerLocationService>>();
            services.AddScoped<IKeyedRepository<PartnerRequest>, KeyedRepository<PartnerRequest>>();
            services.AddScoped<ILookupRepository<PartnerRequestStatus>, LookupRepository<PartnerRequestStatus>>();
            services
                .AddScoped<IKeyedRepository<PartnerSocialMediaAccount>, KeyedRepository<PartnerSocialMediaAccount>>();
            services.AddScoped<ILookupRepository<PartnerStatus>, LookupRepository<PartnerStatus>>();
            services.AddScoped<ILookupRepository<PartnerType>, LookupRepository<PartnerType>>();
            services.AddScoped<IKeyedRepository<PickupLocation>, KeyedRepository<PickupLocation>>();
            services.AddScoped<ILookupRepository<ServiceType>, LookupRepository<ServiceType>>();
            services.AddScoped<ILookupRepository<SocialMediaAccountType>, LookupRepository<SocialMediaAccountType>>();
            services.AddScoped<IKeyedRepository<User>, KeyedRepository<User>>();
            services.AddScoped<IKeyedRepository<UserNotification>, KeyedRepository<UserNotification>>();
            services.AddScoped<IKeyedRepository<Waiver>, KeyedRepository<Waiver>>();
            services.AddScoped<ILookupRepository<WeightUnit>, LookupRepository<WeightUnit>>();

            // Team repositories
            services.AddScoped<IKeyedRepository<Team>, KeyedRepository<Team>>();
            services.AddScoped<IKeyedRepository<TeamMember>, KeyedRepository<TeamMember>>();
            services.AddScoped<IKeyedRepository<TeamJoinRequest>, KeyedRepository<TeamJoinRequest>>();
            services.AddScoped<IKeyedRepository<TeamEvent>, KeyedRepository<TeamEvent>>();
            services.AddScoped<IKeyedRepository<TeamPhoto>, KeyedRepository<TeamPhoto>>();

            // Partner Photo repository
            services.AddScoped<IKeyedRepository<PartnerPhoto>, KeyedRepository<PartnerPhoto>>();

            // Event Photo repository
            services.AddScoped<IKeyedRepository<EventPhoto>, KeyedRepository<EventPhoto>>();

            // Adoption repositories
            services.AddScoped<IKeyedRepository<AdoptableArea>, KeyedRepository<AdoptableArea>>();
            services.AddScoped<IKeyedRepository<TeamAdoption>, KeyedRepository<TeamAdoption>>();
            services.AddScoped<IKeyedRepository<TeamAdoptionEvent>, KeyedRepository<TeamAdoptionEvent>>();

            // Area generation repositories
            services.AddScoped<IKeyedRepository<AreaGenerationBatch>, KeyedRepository<AreaGenerationBatch>>();
            services.AddScoped<IKeyedRepository<StagedAdoptableArea>, KeyedRepository<StagedAdoptableArea>>();

            // Sponsored adoption repositories
            services.AddScoped<IKeyedRepository<Sponsor>, KeyedRepository<Sponsor>>();
            services.AddScoped<IKeyedRepository<ProfessionalCompany>, KeyedRepository<ProfessionalCompany>>();
            services.AddScoped<IBaseRepository<ProfessionalCompanyUser>, BaseRepository<ProfessionalCompanyUser>>();
            services.AddScoped<IKeyedRepository<SponsoredAdoption>, KeyedRepository<SponsoredAdoption>>();
            services.AddScoped<IKeyedRepository<ProfessionalCleanupLog>, KeyedRepository<ProfessionalCleanupLog>>();

            // User Feedback repository
            services.AddScoped<IKeyedRepository<UserFeedback>, KeyedRepository<UserFeedback>>();

            // Waiver V3 repositories
            services.AddScoped<IKeyedRepository<WaiverVersion>, KeyedRepository<WaiverVersion>>();
            services.AddScoped<IBaseRepository<CommunityWaiver>, BaseRepository<CommunityWaiver>>();
            services.AddScoped<IKeyedRepository<UserWaiver>, KeyedRepository<UserWaiver>>();

            // Email Invite repositories
            services.AddScoped<IKeyedRepository<EmailInviteBatch>, KeyedRepository<EmailInviteBatch>>();
            services.AddScoped<IKeyedRepository<EmailInvite>, KeyedRepository<EmailInvite>>();

            // Community Prospect repositories
            services.AddScoped<IKeyedRepository<CommunityProspect>, KeyedRepository<CommunityProspect>>();
            services.AddScoped<IKeyedRepository<ProspectActivity>, KeyedRepository<ProspectActivity>>();
            services.AddScoped<IKeyedRepository<ProspectOutreachEmail>, KeyedRepository<ProspectOutreachEmail>>();

            // Newsletter repositories
            services.AddScoped<IKeyedRepository<Newsletter>, KeyedRepository<Newsletter>>();
            services.AddScoped<ILookupRepository<NewsletterCategory>, LookupRepository<NewsletterCategory>>();
            services.AddScoped<ILookupRepository<NewsletterTemplate>, LookupRepository<NewsletterTemplate>>();

            return services;
        }
    }
}