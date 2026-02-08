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
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;

    private IEnumerable<TrashMob.Models.Poco.Location> locations = [];
    private IEnumerable<Event> allEvents = [];
    private IEnumerable<Event> rawEvents = [];

    private string? selectedCity;
    private string? selectedCountry;
    private string? selectedRegion;

    [ObservableProperty]
    private AddressViewModel userLocation = new();

    [ObservableProperty]
    private bool isMapSelected;

    [ObservableProperty]
    private bool isListSelected;

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

    public ObservableCollection<string> CountryCollection { get; } = [];

    public ObservableCollection<string> RegionCollection { get; } = [];

    public ObservableCollection<string> CityCollection { get; } = [];

    public string? SelectedCountry
    {
        get => selectedCountry;
        set
        {
            selectedCountry = value;
            OnPropertyChanged();
            HandleCountrySelected();
        }
    }

    public string? SelectedRegion
    {
        get => selectedRegion;
        set
        {
            selectedRegion = value;
            OnPropertyChanged();
            HandleRegionSelected();
        }
    }

    public string? SelectedCity
    {
        get => selectedCity;
        set
        {
            selectedCity = value;
            OnPropertyChanged();
            RebuildAddresses();
        }
    }

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
    private Task MapSelected()
    {
        IsMapSelected = true;
        IsListSelected = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ListSelected()
    {
        IsMapSelected = false;
        IsListSelected = true;
        return Task.CompletedTask;
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

        SelectedCountry = null;
        SelectedRegion = null;
        SelectedCity = null;

        await Task.WhenAll(
            RefreshEvents(),
            RefreshLitterReports());

        RebuildAddresses();

        IsBusy = false;
    }

    private async Task RefreshEvents()
    {
        var startDate = DateTimeOffset.UtcNow;
        var endDate = DateTimeOffset.UtcNow.AddMonths(3);

        locations = await mobEventManager.GetLocationsByTimeRangeAsync(startDate, endDate);

        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        if (locations != null && locations.Any())
        {
            foreach (var country in locations.Select(l => l.Country).Distinct())
            {
                if (!string.IsNullOrEmpty(country))
                {
                    CountryCollection.Add(country);
                }
            }
        }

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

            if (!string.IsNullOrEmpty(selectedCountry))
            {
                filtered = filtered.Where(e => e.Country == selectedCountry);
            }

            if (!string.IsNullOrEmpty(selectedRegion))
            {
                filtered = filtered.Where(e => e.Region == selectedRegion);
            }

            if (!string.IsNullOrEmpty(selectedCity))
            {
                filtered = filtered.Where(e => e.City == selectedCity);
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

    private void HandleCountrySelected()
    {
        selectedRegion = null;
        OnPropertyChanged(nameof(SelectedRegion));
        selectedCity = null;
        OnPropertyChanged(nameof(SelectedCity));

        RegionCollection.Clear();

        if (locations != null && !string.IsNullOrEmpty(selectedCountry))
        {
            foreach (var region in locations.Where(l => l.Country == selectedCountry).Select(l => l.Region).Distinct())
            {
                if (!string.IsNullOrEmpty(region))
                {
                    RegionCollection.Add(region);
                }
            }
        }

        RebuildAddresses();
    }

    private void HandleRegionSelected()
    {
        selectedCity = null;
        OnPropertyChanged(nameof(SelectedCity));

        CityCollection.Clear();

        if (locations != null && !string.IsNullOrEmpty(selectedCountry) && !string.IsNullOrEmpty(selectedRegion))
        {
            foreach (var city in locations.Where(l => l.Country == selectedCountry && l.Region == selectedRegion).Select(l => l.City).Distinct())
            {
                if (!string.IsNullOrEmpty(city))
                {
                    CityCollection.Add(city);
                }
            }
        }

        RebuildAddresses();
    }
}
