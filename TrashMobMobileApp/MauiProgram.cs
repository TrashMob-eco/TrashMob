namespace TrashMobMobileApp;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TrashMobMobileApp.Extensions;
using TrashMobMobileApp.Config;
using CommunityToolkit.Maui;
using Sentry;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

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

        string strAppConfigStreamName = string.Empty;

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        strAppConfigStreamName = "TrashMobMobileApp.appSettings.Development.json";
#else
        strAppConfigStreamName = "TrashMobMobileApp.appSettings.json"; 
#endif

        var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
        var stream = assembly.GetManifestResourceStream(strAppConfigStreamName);
        builder.Configuration.AddJsonStream(stream);

        builder.Services.AddMudblazorServices();
        builder.Services.AddStateContainers();
        builder.Services.Configure<Settings>(options => builder.Configuration.GetSection("Settings").Bind(options));
        builder.Services.AddTrashMobServices(builder.Configuration);
        builder.Services.AddRestClientServices(builder.Configuration);
        builder.Services.AddSingleton<MainView>();
        builder.Services.AddTransient<AppHost>();

        builder.Services.AddLogging();
        builder.Services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();
        builder.UseMauiMaps();

        return builder.Build();
    }
}

public class CustomBoundaryLogger : IErrorBoundaryLogger
{
    public ValueTask LogErrorAsync(Exception exception)
    {
        SentrySdk.CaptureException(exception);
        return ValueTask.CompletedTask;
    }
}
