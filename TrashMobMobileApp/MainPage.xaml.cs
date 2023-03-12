namespace TrashMobMobileApp;

using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.StateContainers;

public partial class MainPage : ContentPage
{
    private readonly AppHost appHost;

    private readonly IB2CAuthenticationService b2CAuthenticationService;
    private readonly IUserManager userManager;
    private readonly UserStateInformation userStateInformation;

    public MainPage(AppHost appHost, IB2CAuthenticationService b2CAuthenticationService, IUserManager userManager, UserStateInformation userStateInformation)
    {
        InitializeComponent();
        this.appHost = appHost;
        this.b2CAuthenticationService = b2CAuthenticationService;
        this.userManager = userManager;
        this.userStateInformation = userStateInformation;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (userStateInformation.OnSignOut == null)
        {
            userStateInformation.OnSignOut += async () => await OnSignOutAsync();
        }

        await PerformAuthenticationAsync();
    }

    private async Task PerformAuthenticationAsync()
    {
        try
        {
            await b2CAuthenticationService.SignInAsync(userManager);

            if (UserState.UserContext.AccessToken != null)
            {
                await Navigation.PushAsync(appHost);
            }
        }
        catch
        {
            await OnSignOutAsync();
        }
    }

    private async Task OnSignOutAsync()
    {
        await b2CAuthenticationService.SignOutAsync();
        await PerformAuthenticationAsync();
    }
}
