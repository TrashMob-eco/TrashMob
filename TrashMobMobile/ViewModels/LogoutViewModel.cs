namespace TrashMobMobile.ViewModels;

using TrashMobMobile.Authentication;
using TrashMobMobile.Services;

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

        try
        {
            await authService.SignOutAsync();

            await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError($"An error has occured while logging out. Please wait and try again in a moment.");
        }   
    }
}