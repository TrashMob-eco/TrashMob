namespace TrashMobMobile.Extensions
{
    using Microsoft.Extensions.Configuration;
    using Polly;
    using Polly.Extensions.Http;
    using TrashMobMobile.Authentication;
    using TrashMobMobile.Config;
    using TrashMobMobile.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTrashMobServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IUserService, UserService>();
            
            services.AddSingleton<INotificationService, NotificationService>();

            services.AddSingleton<IAchievementManager, AchievementManager>();
            services.AddSingleton<IAchievementRestService, AchievementRestService>();
            services.AddSingleton<ICommunityManager, CommunityManager>();
            services.AddSingleton<ICommunityRestService, CommunityRestService>();
            services.AddSingleton<IContactRequestManager, ContactRequestManager>();
            services.AddSingleton<IContactRequestRestService, ContactRequestRestService>();
            services.AddSingleton<IEventAttendeeRestService, EventAttendeeRestService>();
            services.AddSingleton<IEventAttendeeMetricsRestService, EventAttendeeMetricsRestService>();
            services.AddSingleton<IEventAttendeeRouteRestService, EventAttendeeRouteRestService>();
            services.AddSingleton<IEventLitterReportManager, EventLitterReportManager>();
            services.AddSingleton<IEventLitterReportRestService, EventLitterReportRestService>();
            services.AddSingleton<IEventPhotoManager, EventPhotoManager>();
            services.AddSingleton<IEventPhotoRestService, EventPhotoRestService>();
            services.AddSingleton<IEventPartnerLocationServiceRestService, EventPartnerLocationServiceRestService>();
            services
                .AddSingleton<IEventPartnerLocationServiceStatusRestService,
                    EventPartnerLocationServiceStatusRestService>();
            services.AddSingleton<IEventSummaryRestService, EventSummaryRestService>();
            services.AddSingleton<IEventTypeRestService, EventTypeRestService>();
            services.AddSingleton<ILeaderboardManager, LeaderboardManager>();
            services.AddSingleton<ILeaderboardRestService, LeaderboardRestService>();
            services.AddSingleton<INewsletterPreferenceManager, NewsletterPreferenceManager>();
            services.AddSingleton<INewsletterPreferenceRestService, NewsletterPreferenceRestService>();
            services.AddSingleton<ILitterReportManager, LitterReportManager>();
            services.AddSingleton<ILitterReportRestService, LitterReportRestService>();
            services.AddSingleton<IMapRestService, MapRestService>();
            services.AddSingleton<IMobEventManager, MobEventManager>();
            services.AddSingleton<IMobEventRestService, MobEventRestService>();
            services.AddSingleton<IPickupLocationManager, PickupLocationManager>();
            services.AddSingleton<IPickupLocationRestService, PickupLocationRestService>();
            services.AddSingleton<IServiceTypeRestService, ServiceTypeRestService>();
            services.AddSingleton<IAppVersionRestService, AppVersionRestService>();
            services.AddSingleton<IAppVersionCheckService, AppVersionCheckService>();
            services.AddSingleton<IStatsRestService, StatsRestService>();
            services.AddSingleton<ITeamManager, TeamManager>();
            services.AddSingleton<ITeamRestService, TeamRestService>();
            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<IUserRestService, UserRestService>();
            services.AddSingleton<IWaiverManager, WaiverManager>();
            services.AddSingleton<IWaiverRestService, WaiverRestService>();
            services.AddSingleton<IRouteTrackingSessionManager, RouteTrackingSessionManager>();

            return services;
        }

        public static IServiceCollection AddRestClientServices(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            /* When running in an emulator localhost would not work as expected.
             * You need to do forwarding, you can use ngrok, check an example before
             * Use your correct FairPlayTube API port
             * */
            //ngrok.exe http https://localhost:44373 -host-header="localhost:44373"
            services.AddScoped<AuthHandler>();
            services.AddHttpClient($"ServerAPI", client =>
                {
                    client.BaseAddress = new Uri(Settings.ApiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(60);
                })
                .AddHttpMessageHandler<AuthHandler>()
                .AddHttpMessageHandler<SentryHttpMessageHandler>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient($"ServerAPI.Anonymous", client =>
                {
                    client.BaseAddress = new Uri(Settings.ApiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(60);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddHttpMessageHandler<SentryHttpMessageHandler>()
                .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}