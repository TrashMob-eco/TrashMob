namespace TrashMobMobileApp;

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Extensions;
using Sentry;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.UseSentry(options =>
        {
            // The DSN is the only required setting.
            options.Dsn = "https://4be7fb697cee47ce9554bb64f7d6a476@o4505460799045632.ingest.sentry.io/4505460800225280";

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
		
		builder.Services.AddHttpClient(AuthConstants.AUTHENTICATED_CLIENT, client =>
		{
			client.BaseAddress = new Uri(AuthConstants.ApiBaseUri);
		}).AddHttpMessageHandler<AuthHandler>();
		
		builder.Services.AddTrashMobServices();
        // builder.Services.AddRestClientServices(builder.Configuration);
        
		// Pages
        builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<WelcomePage>();

		// ViewModels
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<WelcomeViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
