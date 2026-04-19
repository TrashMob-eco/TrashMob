namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

public partial class DependentWaiverViewModel(
    IDependentRestService dependentRestService,
    IWaiverManager waiverManager,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    private Guid dependentId;
    private Guid selectedWaiverVersionId;

    [ObservableProperty]
    private string dependentName = string.Empty;

    [ObservableProperty]
    private string eventName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DependentWaiverItem> waivers = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListVisible))]
    private bool isSigningWaiver;

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

    [ObservableProperty]
    private bool allWaiversSigned;

    [ObservableProperty]
    private string progressText = string.Empty;

    public bool IsListVisible => !IsSigningWaiver;

    private bool CanSignWaiver => HasAgreed && TypedLegalName.Trim().Length >= 2;

    public async Task Init(Guid depId, string depName, string evtName, string waiverVersionIdsCsv)
    {
        dependentId = depId;
        DependentName = depName;
        EventName = evtName;
        IsSigningWaiver = false;
        AllWaiversSigned = false;

        await ExecuteAsync(async () =>
        {
            var versionIds = waiverVersionIdsCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToList();

            var requiredWaivers = await waiverManager.GetRequiredWaiversAsync();
            var items = new ObservableCollection<DependentWaiverItem>();

            foreach (var id in versionIds)
            {
                var waiver = requiredWaivers.Find(w => w.Id == id);
                items.Add(new DependentWaiverItem
                {
                    WaiverVersionId = id,
                    Name = waiver?.Name ?? "Waiver",
                    WaiverText = waiver?.WaiverText ?? string.Empty,
                    IsSigned = false,
                });
            }

            Waivers = items;
            UpdateProgress();

            // If only one waiver, go straight to signing
            if (Waivers.Count == 1)
            {
                OpenWaiverForSigning(Waivers[0]);
            }
        }, "Failed to load waiver details. Please try again.");
    }

    [RelayCommand]
    private void SelectWaiver(DependentWaiverItem item)
    {
        if (item == null || item.IsSigned) return;
        OpenWaiverForSigning(item);
    }

    private void OpenWaiverForSigning(DependentWaiverItem item)
    {
        selectedWaiverVersionId = item.WaiverVersionId;
        WaiverName = item.Name;
        WaiverText = item.WaiverText;
        TypedLegalName = string.Empty;
        HasAgreed = false;
        IsSigningWaiver = true;
    }

    [RelayCommand(CanExecute = nameof(CanSignWaiver))]
    private async Task SignWaiver()
    {
        await ExecuteAsync(async () =>
        {
            await dependentRestService.SignWaiverAsync(dependentId, selectedWaiverVersionId, TypedLegalName.Trim());

            var item = Waivers.FirstOrDefault(w => w.WaiverVersionId == selectedWaiverVersionId);
            if (item != null) item.IsSigned = true;

            UpdateProgress();

            if (AllWaiversSigned)
            {
                await NotificationService.Notify($"All waivers signed for {DependentName}.");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                IsSigningWaiver = false;
            }
        }, "Failed to sign waiver. Please try again.");
    }

    [RelayCommand]
    private void BackToList()
    {
        IsSigningWaiver = false;
    }

    [RelayCommand]
    private async Task Done()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void UpdateProgress()
    {
        var signed = Waivers.Count(w => w.IsSigned);
        var total = Waivers.Count;
        AllWaiversSigned = signed == total;
        ProgressText = $"{signed} of {total} waiver(s) signed";
    }
}

public class DependentWaiverItem
{
    public Guid WaiverVersionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string WaiverText { get; set; } = string.Empty;

    public bool IsSigned { get; set; }

    public string StatusText => IsSigned ? "Signed" : "Needs Signature";
}
