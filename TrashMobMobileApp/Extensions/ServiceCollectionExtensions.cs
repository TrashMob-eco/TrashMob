namespace TrashMobMobileApp.Extensions
{
    using Microsoft.Extensions.Configuration;
    using MudBlazor;
    using MudBlazor.Services;
    using Polly;
    using Polly.Extensions.Http;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Config;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.StateContainers;

    public static class ServiceCollectionExtensions
    {
        public const string ASSEMBLY_NAME = "TrashMobMobileApp";

        public static IServiceCollection AddMudblazorServices(this IServiceCollection services)
        {
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 3000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
            });

            return services;
        }

        public static IServiceCollection AddStateContainers(this IServiceCollection services)
        {
            services.AddScoped<PageTitleContainer>();
            services.AddScoped<UserStateInformation>();
            services.AddSingleton<EventStateInformation>();
            return services;
        }

        public static IServiceCollection AddTrashMobServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IContactRequestManager, ContactRequestManager>();
            services.AddSingleton<IContactRequestRestService, ContactRequestRestService>();
            services.AddSingleton<IDocusignRestService, DocusignRestService>();
            services.AddSingleton<IEventAttendeeRestService, EventAttendeeRestService>();
            services.AddSingleton<IEventPartnerLocationServiceRestService, EventPartnerLocationServiceRestService>();
            services.AddSingleton<IEventPartnerLocationServiceStatusRestService, EventPartnerLocationServiceStatusRestService>();
            services.AddSingleton<IEventSummaryRestService, EventSummaryRestService>();
            services.AddSingleton<IEventTypeRestService, EventTypeRestService>();
            services.AddSingleton<IMapRestService, MapRestService>();
            services.AddSingleton<IMobEventManager, MobEventManager>();
            services.AddSingleton<IMobEventRestService, MobEventRestService>();
            services.AddSingleton<IPickupLocationManager, PickupLocationManager>();
            services.AddSingleton<IPickupLocationRestService, PickupLocationRestService>();
            services.AddSingleton<IServiceTypeRestService, ServiceTypeRestService>();
            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<IUserRestService, UserRestService>();
            services.AddSingleton<IWaiverManager, WaiverManager>();
            services.AddSingleton<IWaiverRestService, WaiverRestService>();
            var settings = configuration.GetSection("Settings").Get<Settings>();
            services.AddSingleton(settings.AzureADB2C);

            return services;
        }

        public static IServiceCollection AddRestClientServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            /* When running in an emulator localhost would not work as expected.
             * You need to do forwarding, you can use ngrok, check an example before
             * Use your correct FairPlayTube API port
             * */
            //ngrok.exe http https://localhost:44373 -host-header="localhost:44373"
            //string fairPlayTubeapiAddress = "REPLACE_WITH_NGROK_GENERATED_URL";
            var settings = configuration.GetSection("Settings").Get<Settings>();
            services.AddScoped<BaseAddressAuthorizationMessageHandler>();
            services.AddHttpClient($"{ASSEMBLY_NAME}.ServerAPI", client =>
                    client.BaseAddress = new Uri(settings.ApiBaseUrl))
                                            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
                                            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                                            .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient($"{ASSEMBLY_NAME}.ServerAPI.Anonymous", client =>
                    client.BaseAddress = new Uri(settings.ApiBaseUrl))
                                            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                                            .AddPolicyHandler(GetRetryPolicy());

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{ASSEMBLY_NAME}.ServerAPI"));

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{ASSEMBLY_NAME}.ServerAPI.Anonymous"));

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
