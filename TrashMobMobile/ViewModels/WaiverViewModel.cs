namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

public partial class WaiverViewModel(INotificationService notificationService, IWaiverManager waiverManager) : BaseViewModel(notificationService)
{
    private Guid waiverVersionId;

    [ObservableProperty]
    private string waiverName = string.Empty;

    [ObservableProperty]
    private string waiverText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignWaiverCommand))]
    private string typedLegalName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignWaiverCommand))]
    private bool hasAgreed;

    private bool CanSignWaiver => HasAgreed && TypedLegalName.Trim().Length >= 2;

    public async Task Init(Guid versionId)
    {
        waiverVersionId = versionId;
        TypedLegalName = string.Empty;
        HasAgreed = false;

        await ExecuteAsync(async () =>
        {
            var requiredWaivers = await waiverManager.GetRequiredWaiversAsync();
            var waiver = requiredWaivers.Find(w => w.Id == versionId);

            if (waiver != null)
            {
                WaiverName = waiver.Name;
                WaiverText = waiver.WaiverText ?? string.Empty;
            }
        }, "Failed to load waiver details. Please try again.");
    }

    [RelayCommand(CanExecute = nameof(CanSignWaiver))]
    private async Task SignWaiver()
    {
        await ExecuteAsync(async () =>
        {
            var request = new AcceptWaiverApiRequest
            {
                WaiverVersionId = waiverVersionId,
                TypedLegalName = TypedLegalName.Trim(),
            };

            await waiverManager.AcceptWaiverAsync(request);
            await Shell.Current.GoToAsync("..");
        }, "An error occurred while signing the waiver. Please try again.");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
