using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Services;

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
		builder.Services.AddSingleton<AuthHandler>();
		
		builder.Services.AddHttpClient(AuthConstants.AUTHENTICATED_CLIENT, client =>
		{
			client.BaseAddress = new Uri(AuthConstants.ApiBaseUri);
		}).AddHttpMessageHandler<AuthHandler>();
		
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IUserService, UserService>();

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
