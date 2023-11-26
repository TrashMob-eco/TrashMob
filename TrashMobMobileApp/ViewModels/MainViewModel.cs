#nullable enable

namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Data;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IUserRestService userRestService;

    public MainViewModel(IAuthService authService, IUserRestService userRestService)
    {
        this.authService = authService;
        this.userRestService = userRestService;
    }

    [ObservableProperty]
    private string? welcomeMessage;

    ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = new ObservableCollection<EventViewModel>();

    public async Task Init()
    {
        IsBusy = true;
        
        var signedIn = await authService.SignInSilentAsync(true);

        if (signedIn.Succeeded)
        {
            var email = authService.GetUserEmail();
            var user = await userRestService.GetUserByEmailAsync(email, UserState.UserContext);
            WelcomeMessage = $"Welcome, {user.UserName}!";
            
            IsBusy = false;
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
