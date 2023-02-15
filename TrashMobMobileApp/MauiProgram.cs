namespace TrashMobMobileApp;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TrashMobMobileApp.Extensions;
using TrashMobMobileApp.Config;
using CommunityToolkit.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

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

        builder.Services.AddLogging();
        builder.Services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();
        builder.UseMauiMaps();

        //AppCenter.Start("android=d044d1b4-6fbc-4547-8fae-d0286d9ccbaa;" +
        //      "ios=0f9bed29-14d0-4e38-a396-64e5cd185d10;",
        //      typeof(Analytics), typeof(Crashes));

        return builder.Build();
    }
}

public class CustomBoundaryLogger : IErrorBoundaryLogger
{
    public ValueTask LogErrorAsync(Exception exception)
    {
        Crashes.TrackError(exception);
        return ValueTask.CompletedTask;
    }
}
