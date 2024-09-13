namespace TrashMobMobile.ViewModels;

using TrashMobMobile.Authentication;
using TrashMobMobile.Services;

public class LogoutViewModel(IAuthService authService, IStatsRestService statsRestService, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IAuthService authService = authService;

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
            await NotificationService.NotifyError($"An error has occurred while logging out. Please wait and try again in a moment.");
        }   
    }
}