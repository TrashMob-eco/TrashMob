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
        await ExecuteAsync(async () =>
        {
            var user = await userManager.GetUserAsync(userManager.CurrentUser.Id.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.DateAgreedToTrashMobWaiver = DateTime.UtcNow;
            user.TrashMobWaiverVersion = Settings.CurrentTrashMobWaiverVersion.VersionId;

            await userManager.UpdateUserAsync(user);
            userManager.CurrentUser = user;
            await Navigation.PopAsync();
        }, "An error occurred while signing the waiver. Please try again.");
    }
}