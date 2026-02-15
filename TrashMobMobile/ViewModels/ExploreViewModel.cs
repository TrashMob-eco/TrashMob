namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Maps;
using Sentry;
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
    private IEnumerable<LitterReport> allLitterReports = [];
    private IEnumerable<LitterReport> rawLitterReports = [];

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

    // Event filter selection — bound via RadioButtonGroup.SelectedValue
    private string eventFilterSelection = "Upcoming";

    public string EventFilterSelection
    {
        get => eventFilterSelection;
        set
        {
            if (value == null || eventFilterSelection == value) return;
            eventFilterSelection = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsUpcomingSelected));
            OnPropertyChanged(nameof(IsCompletedSelected));
            HandleEventFilterChanged();
        }
    }

    public bool IsUpcomingSelected => EventFilterSelection == "Upcoming";

    public bool IsCompletedSelected => EventFilterSelection == "Completed";

    public ObservableCollection<string> UpcomingDateRanges { get; } = [];

    public ObservableCollection<string> CompletedDateRanges { get; } = [];

    private string selectedUpcomingDateRange = DateRanges.ThisMonth;

    public string SelectedUpcomingDateRange
    {
        get => selectedUpcomingDateRange;
        set
        {
            if (value == null) return;
            if (selectedUpcomingDateRange != value)
            {
                selectedUpcomingDateRange = value;
                OnPropertyChanged();
                HandleEventDateRangeChanged();
            }
        }
    }

    private string selectedCompletedDateRange = DateRanges.LastMonth;

    public string SelectedCompletedDateRange
    {
        get => selectedCompletedDateRange;
        set
        {
            if (value == null) return;
            if (selectedCompletedDateRange != value)
            {
                selectedCompletedDateRange = value;
                OnPropertyChanged();
                HandleEventDateRangeChanged();
            }
        }
    }

    // Litter report filter selection — bound via RadioButtonGroup.SelectedValue
    private string litterFilterSelection = "New";

    public string LitterFilterSelection
    {
        get => litterFilterSelection;
        set
        {
            if (value == null || litterFilterSelection == value) return;
            litterFilterSelection = value;
            OnPropertyChanged();
            HandleLitterFilterChanged();
        }
    }

    public ObservableCollection<string> CreatedDateRanges { get; } = [];

    private string selectedCreatedDateRange = DateRanges.LastMonth;

    public string SelectedCreatedDateRange
    {
        get => selectedCreatedDateRange;
        set
        {
            if (value == null) return;
            if (selectedCreatedDateRange != value)
            {
                selectedCreatedDateRange = value;
                OnPropertyChanged();
                HandleLitterDateRangeChanged();
            }
        }
    }

    // User location properties for map centering and travel radius
    [ObservableProperty]
    private bool hasUserLocation;

    public MapSpan? UserMapSpan { get; private set; }

    public ObservableCollection<EventViewModel> Events { get; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; } = [];

    public ObservableCollection<AddressViewModel> Addresses { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            IsMapSelected = true;
            IsListSelected = false;
            eventFilterSelection = "Upcoming";
            OnPropertyChanged(nameof(EventFilterSelection));
            OnPropertyChanged(nameof(IsUpcomingSelected));
            litterFilterSelection = "New";
            OnPropertyChanged(nameof(LitterFilterSelection));

            UserLocation = userManager.CurrentUser.GetAddress();

            // Populate date range collections (clear first to avoid duplicates on re-navigation)
            if (UpcomingDateRanges.Count == 0)
            {
                foreach (var date in DateRanges.UpcomingRangeDictionary)
                {
                    UpcomingDateRanges.Add(date.Key);
                }
            }

            if (CompletedDateRanges.Count == 0)
            {
                foreach (var date in DateRanges.CompletedRangeDictionary)
                {
                    CompletedDateRanges.Add(date.Key);
                }
            }

            if (CreatedDateRanges.Count == 0)
            {
                foreach (var date in DateRanges.CreatedDateRangeDictionary)
                {
                    CreatedDateRanges.Add(date.Key);
                }
            }

            // Compute user map span for centering
            ComputeUserMapSpan();

            await Task.WhenAll(
                RefreshEvents(),
                RefreshLitterReports());

            RebuildAddresses();
        }, "Failed to load explore data. Please try again.");
    }

    private void ComputeUserMapSpan()
    {
        if (UserLocation.Latitude is not null and not 0 && UserLocation.Longitude is not null and not 0)
        {
            var center = new Microsoft.Maui.Devices.Sensors.Location(UserLocation.Latitude.Value, UserLocation.Longitude.Value);
            var radius = GetTravelRadius();
            UserMapSpan = MapSpan.FromCenterAndRadius(center, radius);
            HasUserLocation = true;
        }
        else
        {
            HasUserLocation = false;
        }
    }

    public Distance GetTravelRadius()
    {
        var user = userManager.CurrentUser;
        var travelLimit = user.TravelLimitForLocalEvents > 0 ? user.TravelLimitForLocalEvents : 10;
        return user.PrefersMetric
            ? Distance.FromKilometers(travelLimit)
            : Distance.FromMiles(travelLimit);
    }

    partial void OnShowEventsChanged(bool value)
    {
        RebuildAddresses();
    }

    partial void OnShowLitterReportsChanged(bool value)
    {
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

    private async void HandleEventFilterChanged()
    {
        try
        {
            IsBusy = true;
            await RefreshEvents();
            RebuildAddresses();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void HandleEventDateRangeChanged()
    {
        try
        {
            IsBusy = true;
            await RefreshEvents();
            RebuildAddresses();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void HandleLitterFilterChanged()
    {
        try
        {
            IsBusy = true;
            await RefreshLitterReports();
            RebuildAddresses();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void HandleLitterDateRangeChanged()
    {
        try
        {
            IsBusy = true;
            await RefreshLitterReports();
            RebuildAddresses();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshEvents()
    {
        DateTimeOffset startDate;
        DateTimeOffset endDate;

        if (IsUpcomingSelected)
        {
            startDate = DateTimeOffset.Now.Date.AddDays(DateRanges.UpcomingRangeDictionary[SelectedUpcomingDateRange].Item1);
            endDate = DateTimeOffset.Now.Date.AddDays(DateRanges.UpcomingRangeDictionary[SelectedUpcomingDateRange].Item2);
        }
        else
        {
            startDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CompletedRangeDictionary[SelectedCompletedDateRange].Item1);
            endDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CompletedRangeDictionary[SelectedCompletedDateRange].Item2);
        }

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

        if (IsUpcomingSelected)
        {
            allEvents = events.Where(e => !e.IsCompleted()).ToList();
        }
        else
        {
            allEvents = events.Where(e => e.IsCompleted()).ToList();
        }

        rawEvents = allEvents;
    }

    private async Task RefreshLitterReports()
    {
        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item1);
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item2);

        var filter = new LitterReportFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            PageIndex = 0,
            PageSize = 1000,
            IncludeLitterImages = true,
        };

        filter.LitterReportStatusId = LitterFilterSelection switch
        {
            "Assigned" => (int)LitterReportStatusEnum.Assigned,
            "Cleaned" => (int)LitterReportStatusEnum.Cleaned,
            _ => (int)LitterReportStatusEnum.New,
        };

        allLitterReports = (await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, true)).ToList();
        rawLitterReports = allLitterReports;
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
        else
        {
            Events.Clear();
        }

        if (ShowLitterReports)
        {
            IEnumerable<LitterReport> filtered = allLitterReports;

            if (!string.IsNullOrEmpty(SelectedCountry))
            {
                filtered = filtered.Where(l => l.LitterImages.Any(i => i.Country == SelectedCountry));
            }

            if (!string.IsNullOrEmpty(SelectedRegion))
            {
                filtered = filtered.Where(l => l.LitterImages.Any(i => i.Region == SelectedRegion));
            }

            if (!string.IsNullOrEmpty(SelectedCity))
            {
                filtered = filtered.Where(l => l.LitterImages.Any(i => i.City == SelectedCity));
            }

            rawLitterReports = filtered;

            LitterReports.Clear();
            foreach (var report in rawLitterReports.OrderByDescending(r => r.CreatedDate))
            {
                var vm = report.ToLitterReportViewModel(NotificationService);
                LitterReports.Add(vm);
                foreach (var image in vm.LitterImageViewModels.Where(i => i.Address.Location != null))
                {
                    Addresses.Add(image.Address);
                }
            }
        }
        else
        {
            LitterReports.Clear();
        }

        AreItemsFound = Addresses.Count > 0;
        AreNoItemsFound = !AreItemsFound;
    }
}
