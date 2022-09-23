namespace TrashMobMobileApp;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TrashMobMobileApp.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

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
        builder.Services.AddTrashMobServices(builder.Configuration);
        builder.Services.AddRestClientServices(builder.Configuration);

        builder.Services.AddLogging();
        builder.Services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();
        
        return builder.Build();
    }
}

public class CustomBoundaryLogger : IErrorBoundaryLogger
{
    public ValueTask LogErrorAsync(Exception exception)
    {
        return ValueTask.CompletedTask;
    }
}
