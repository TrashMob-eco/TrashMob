namespace TrashMobMobile.Extensions
{
    using Microsoft.Extensions.Configuration;
    using Polly;
    using Polly.Extensions.Http;
    using TrashMobMobile.Authentication;
    using TrashMobMobile.Config;
    using TrashMobMobile.Data;
    using TrashMobMobile.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTrashMobServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IContactRequestManager, ContactRequestManager>();
            services.AddSingleton<IContactRequestRestService, ContactRequestRestService>();
            services.AddSingleton<IDocusignRestService, DocusignRestService>();
            services.AddSingleton<IEventAttendeeRestService, EventAttendeeRestService>();
            services.AddSingleton<IEventPartnerLocationServiceRestService, EventPartnerLocationServiceRestService>();
            services
                .AddSingleton<IEventPartnerLocationServiceStatusRestService,
                    EventPartnerLocationServiceStatusRestService>();
            services.AddSingleton<IEventSummaryRestService, EventSummaryRestService>();
            services.AddSingleton<IEventTypeRestService, EventTypeRestService>();
            services.AddSingleton<ILitterReportManager, LitterReportManager>();
            services.AddSingleton<ILitterReportRestService, LitterReportRestService>();
            services.AddSingleton<IMapRestService, MapRestService>();
            services.AddSingleton<IMobEventManager, MobEventManager>();
            services.AddSingleton<IMobEventRestService, MobEventRestService>();
            services.AddSingleton<IPickupLocationManager, PickupLocationManager>();
            services.AddSingleton<IPickupLocationRestService, PickupLocationRestService>();
            services.AddSingleton<IServiceTypeRestService, ServiceTypeRestService>();
            services.AddSingleton<IStatsRestService, StatsRestService>();
            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<IUserRestService, UserRestService>();
            services.AddSingleton<IWaiverManager, WaiverManager>();
            services.AddSingleton<IWaiverRestService, WaiverRestService>();

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
            services.AddScoped<BaseAddressAuthorizationMessageHandler>();
            services.AddHttpClient($"ServerAPI", client =>
                    client.BaseAddress = new Uri(Settings.ApiBaseUrl))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient($"ServerAPI.Anonymous", client =>
                    client.BaseAddress = new Uri(Settings.ApiBaseUrl))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}