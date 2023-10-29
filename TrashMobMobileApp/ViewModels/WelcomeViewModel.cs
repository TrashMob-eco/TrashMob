using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobileApp.Authentication;

namespace TrashMobMobileApp.ViewModels;

public partial class WelcomeViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public WelcomeViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;

    [RelayCommand]
    private async Task SignIn()
    {
        IsBusy = true;

        var signedIn = await _authService.SignInAsync();

        IsBusy = false;

        if (signedIn.Succeeded)
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            IsError = true;
        }
    }
}
