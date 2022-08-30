namespace TrashMobMobileApp;

using Microsoft.AspNetCore.Components.WebView.Maui;
using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
		
        // Add Services
        builder.Services.AddSingleton<IB2CAuthenticationService, B2CAuthenticationService>();
        builder.Services.AddSingleton<IContactRequestManager, ContactRequestManager>();
        builder.Services.AddSingleton<IContactRequestRestService, ContactRequestRestService>();
        builder.Services.AddSingleton<IDataStore<Item>, MockDataStore>();
        builder.Services.AddSingleton<IEventAttendeeRestService, EventAttendeeRestService>();
        builder.Services.AddSingleton<IEventSummaryRestService, EventSummaryRestService>();
        builder.Services.AddSingleton<IEventTypeRestService, EventTypeRestService>();
        builder.Services.AddSingleton<IMapRestService, MapRestService>();
        builder.Services.AddSingleton<IMobEventManager, MobEventManager>();
        builder.Services.AddSingleton<IMobEventRestService, MobEventRestService>();
        builder.Services.AddSingleton<IUserManager, UserManager>();
        builder.Services.AddSingleton<IUserRestService, UserRestService>();
        return builder.Build();
	}
}
