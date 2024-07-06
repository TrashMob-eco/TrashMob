namespace TrashMobMobile;

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using TrashMobMobile.Authentication;
using TrashMobMobile.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .ConfigureSyncfusionCore()
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Lexend-Regular.ttf", "LexendRegular");
                fonts.AddFont("Lexend-SemiBold.ttf", "LexendSemibold");
                fonts.AddFont("feather.ttf", "Icons");
                fonts.AddFont("googlematerialdesignicons-webfont.ttf", "GoogleMaterialFontFamily");
            });

        builder.UseSentry(options =>
        {
            // The DSN is the only required setting.
            options.Dsn =
                "https://4be7fb697cee47ce9554bb64f7d6a476@o4505460799045632.ingest.sentry.io/4505460800225280";

            // Use debug mode if you want to see what the SDK is doing.
            // Debug messages are written to stdout with Console.Writeline,
            // and are viewable in your IDE's debug console or with 'adb logcat', etc.
            // This option is not recommended when deploying your application.
            options.Debug = false;

            // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
            // We recommend adjusting this value in production.
            options.TracesSampleRate = 1.0;

            // Other Sentry options can be set here.
        });

        // Services
        builder.Services.AddSingleton<AuthHandler>();

        builder.Services
            .AddHttpClient(AuthConstants.AuthenticatedClient,
                client => { client.BaseAddress = new Uri(AuthConstants.ApiBaseUri); })
            .AddHttpMessageHandler<AuthHandler>();

        builder.Services.AddTrashMobServices();
        builder.Services.AddRestClientServices(builder.Configuration);

        // Pages
        builder.Services.AddTransient<CreatePickupLocationPage>();
        builder.Services.AddTransient<CancelEventPage>();
        builder.Services.AddTransient<ContactUsPage>();
        builder.Services.AddTransient<CreateEventPage>();
        builder.Services.AddTransient<CreateLitterReportPage>();
        builder.Services.AddTransient<EditEventPage>();
        builder.Services.AddTransient<EditEventPartnerLocationServicesPage>();
        builder.Services.AddTransient<EditEventSummaryPage>();
        builder.Services.AddTransient<EditLitterReportPage>();
        builder.Services.AddTransient<EditPickupLocationPage>();
        builder.Services.AddTransient<LogoutPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<ManageEventPartnersPage>();
        builder.Services.AddTransient<MyDashboardPage>();
        builder.Services.AddTransient<SearchEventsPage>();
        builder.Services.AddTransient<SearchLitterReportsPage>();
        builder.Services.AddTransient<SetUserLocationPreferencePage>();
        builder.Services.AddTransient<ViewEventPage>();
        builder.Services.AddTransient<ViewEventSummaryPage>();
        builder.Services.AddTransient<ViewLitterReportPage>();
        builder.Services.AddTransient<ViewPickupLocationPage>();
        builder.Services.AddTransient<WaiverPage>();
        builder.Services.AddTransient<WelcomePage>();

        // ViewModels
        builder.Services.AddTransient<CreatePickupLocationViewModel>();
        builder.Services.AddTransient<CancelEventViewModel>();
        builder.Services.AddTransient<ContactUsViewModel>();
        builder.Services.AddTransient<CreateEventViewModel>();
        builder.Services.AddTransient<CreateLitterReportViewModel>();
        builder.Services.AddTransient<EditEventViewModel>();
        builder.Services.AddTransient<EditEventPartnerLocationServicesViewModel>();
        builder.Services.AddTransient<EditEventSummaryViewModel>();
        builder.Services.AddTransient<EditLitterReportViewModel>();
        builder.Services.AddTransient<EditPickupLocationViewModel>();
        builder.Services.AddTransient<EventSummaryViewModel>();
        builder.Services.AddTransient<LogoutViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<ManageEventPartnersViewModel>();
        builder.Services.AddTransient<MyDashboardViewModel>();
        builder.Services.AddTransient<SearchEventsViewModel>();
        builder.Services.AddTransient<SearchLitterReportsViewModel>();
        builder.Services.AddTransient<SocialMediaShareViewModel>();
        builder.Services.AddTransient<UserLocationPreferenceViewModel>();
        builder.Services.AddTransient<ViewEventViewModel>();
        builder.Services.AddTransient<ViewEventSummaryViewModel>();
        builder.Services.AddTransient<ViewLitterReportViewModel>();
        builder.Services.AddTransient<ViewPickupLocationViewModel>();
        builder.Services.AddTransient<WaiverViewModel>();
        builder.Services.AddTransient<WelcomeViewModel>();

#if USETEST
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}