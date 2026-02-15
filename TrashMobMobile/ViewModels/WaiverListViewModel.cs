namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class WaiverListViewModel(IWaiverManager waiverManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private ObservableCollection<WaiverVersion> pendingWaivers = [];

    [ObservableProperty]
    private ObservableCollection<UserWaiver> signedWaivers = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private bool allWaiversSigned;

    [ObservableProperty]
    private bool hasPendingWaivers;

    [ObservableProperty]
    private bool hasSignedWaivers;

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var pending = await waiverManager.GetRequiredWaiversAsync();
            var signed = await waiverManager.GetMyWaiversAsync();

            PendingWaivers = new ObservableCollection<WaiverVersion>(pending);
            SignedWaivers = new ObservableCollection<UserWaiver>(signed);

            HasPendingWaivers = PendingWaivers.Count > 0;
            HasSignedWaivers = SignedWaivers.Count > 0;
            AllWaiversSigned = PendingWaivers.Count == 0;
        }, "Failed to load waivers. Please try again.");
    }

    [RelayCommand]
    private async Task SelectWaiver(WaiverVersion waiver)
    {
        await Shell.Current.GoToAsync($"{nameof(WaiverPage)}?WaiverVersionId={waiver.Id}");
    }

    [RelayCommand(CanExecute = nameof(AllWaiversSigned))]
    private void Continue()
    {
        Shell.Current.SendBackButtonPressed();
    }
}
