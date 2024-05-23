namespace TrashMobMobile.ViewModels;

using TrashMobMobile.Authentication;
using TrashMobMobile.Data;

public class LogoutViewModel : BaseViewModel
{
    private readonly IAuthService authService;

    public LogoutViewModel(IAuthService authService, IStatsRestService statsRestService)
    {
        this.authService = authService;
    }

    public async Task Init()
    {
        IsBusy = true;

        await authService.SignOutAsync();

        await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");

        IsBusy = false;
    }
}