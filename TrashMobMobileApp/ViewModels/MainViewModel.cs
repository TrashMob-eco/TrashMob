using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using TrashMobMobileApp.Authentication;

namespace TrashMobMobileApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public MainViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    private string? welcomeMessage;

    public async Task Init()
    {
        var signedIn = await _authService.SignInSilentAsync(false);

        if (signedIn.Succeeded)
        {
            // TODO: Add logic to get user name

            WelcomeMessage = "Welcome, Matt!";
        }
        else
        {
            try
            {
                await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
