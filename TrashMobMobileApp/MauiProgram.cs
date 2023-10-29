using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TrashMobMobileApp.Authentication;

namespace TrashMobMobileApp;

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
		
		// Services
		builder.Services.AddSingleton<IAuthService, AuthService>();

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
