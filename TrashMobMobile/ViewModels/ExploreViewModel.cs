namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ExploreViewModel(
    IMobEventManager mobEventManager,
    ILitterReportManager litterReportManager,
    INotificationService notificationService,
    IUserManager userManager) : LocationFilterViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;

    private IEnumerable<Event> allEvents = [];
    private IEnumerable<Event> rawEvents = [];

    [ObservableProperty]
    private AddressViewModel userLocation = new();

    [ObservableProperty]
    private bool showEvents = true;

    [ObservableProperty]
    private bool showLitterReports = true;

    [ObservableProperty]
    private bool areItemsFound;

    [ObservableProperty]
    private bool areNoItemsFound;

    public ObservableCollection<EventViewModel> Events { get; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; } = [];

    public ObservableCollection<AddressViewModel> Addresses { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            IsMapSelected = true;
            IsListSelected = false;
            UserLocation = userManager.CurrentUser.GetAddress();

            await Task.WhenAll(
                RefreshEvents(),
                RefreshLitterReports());

            RebuildAddresses();
        }, "Failed to load explore data. Please try again.");
    }

    [RelayCommand]
    private void ToggleEvents()
    {
        ShowEvents = !ShowEvents;
        RebuildAddresses();
    }

    [RelayCommand]
    private void ToggleLitterReports()
    {
        ShowLitterReports = !ShowLitterReports;
        RebuildAddresses();
    }

    [RelayCommand]
    private async Task ViewEvent(EventViewModel? eventVm)
    {
        if (eventVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewEventPage)}?EventId={eventVm.Id}");
    }

    [RelayCommand]
    private async Task ViewLitterReport(LitterReportViewModel? reportVm)
    {
        if (reportVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewLitterReportPage)}?LitterReportId={reportVm.Id}");
    }

    [RelayCommand]
    private async Task ClearSelections()
    {
        IsBusy = true;

        ClearLocationSelections();

        await Task.WhenAll(
            RefreshEvents(),
            RefreshLitterReports());

        RebuildAddresses();

        IsBusy = false;
    }

    protected override void ApplyFilters()
    {
        RebuildAddresses();
    }

    private async Task RefreshEvents()
    {
        var startDate = DateTimeOffset.UtcNow;
        var endDate = DateTimeOffset.UtcNow.AddMonths(3);

        Locations = await mobEventManager.GetLocationsByTimeRangeAsync(startDate, endDate);

        PopulateCountries();

        var eventFilter = new EventFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            PageIndex = 0,
            PageSize = 100,
        };

        var events = await mobEventManager.GetFilteredEventsAsync(eventFilter);
        allEvents = events.Where(e => !e.IsCompleted()).ToList();
        rawEvents = allEvents;

        Events.Clear();
        foreach (var mobEvent in rawEvents.OrderBy(e => e.EventDate))
        {
            Events.Add(mobEvent.ToEventViewModel(userManager.CurrentUser.Id));
        }
    }

    private async Task RefreshLitterReports()
    {
        var filter = new LitterReportFilter
        {
            StartDate = DateTimeOffset.UtcNow.AddMonths(-6),
            EndDate = DateTimeOffset.UtcNow,
        };

        var reports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, true);

        LitterReports.Clear();
        foreach (var report in reports.OrderByDescending(r => r.CreatedDate))
        {
            LitterReports.Add(report.ToLitterReportViewModel(NotificationService));
        }
    }

    private void RebuildAddresses()
    {
        Addresses.Clear();

        if (ShowEvents)
        {
            IEnumerable<Event> filtered = allEvents;

            if (!string.IsNullOrEmpty(SelectedCountry))
            {
                filtered = filtered.Where(e => e.Country == SelectedCountry);
            }

            if (!string.IsNullOrEmpty(SelectedRegion))
            {
                filtered = filtered.Where(e => e.Region == SelectedRegion);
            }

            if (!string.IsNullOrEmpty(SelectedCity))
            {
                filtered = filtered.Where(e => e.City == SelectedCity);
            }

            rawEvents = filtered;

            Events.Clear();
            foreach (var mobEvent in rawEvents.OrderBy(e => e.EventDate))
            {
                var vm = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);
                Events.Add(vm);
                Addresses.Add(vm.Address);
            }
        }

        if (ShowLitterReports)
        {
            foreach (var report in LitterReports)
            {
                foreach (var image in report.LitterImageViewModels)
                {
                    Addresses.Add(image.Address);
                }
            }
        }

        AreItemsFound = Addresses.Count > 0;
        AreNoItemsFound = !AreItemsFound;
    }
}
