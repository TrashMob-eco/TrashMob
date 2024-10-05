namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Services;

public partial class WaiverViewModel(INotificationService notificationService, IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IUserManager userManager = userManager;

    [RelayCommand]
    private async Task SignWaiver()
    {
        IsBusy = true;

        try
        {
            var user = await userManager.GetUserAsync(App.CurrentUser.Id.ToString()); 
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.DateAgreedToTrashMobWaiver = DateTime.UtcNow;
            user.TrashMobWaiverVersion = Settings.CurrentTrashMobWaiverVersion.VersionId;

            await userManager.UpdateUserAsync(user);
            
            App.CurrentUser = user;
            
            await Navigation.PopAsync();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while opening the waiver page. Please try again.");
        }
    }
}