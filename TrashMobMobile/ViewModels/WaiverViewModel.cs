namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Services;

public partial class WaiverViewModel(INotificationService notificationService, IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignWaiverCommand))]
    private string typedLegalName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignWaiverCommand))]
    private bool hasAgreed;

    private bool CanSignWaiver => HasAgreed && TypedLegalName.Trim().Length >= 2;

    [RelayCommand(CanExecute = nameof(CanSignWaiver))]
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