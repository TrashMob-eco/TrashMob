namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class SearchEventsViewModel(IMobEventManager mobEventManager, 
                                           INotificationService notificationService,
                                           IUserManager userManager)
    : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IUserManager userManager = userManager;

    private IEnumerable<TrashMob.Models.Poco.Location> locations = [];

    private string? selectedCity;
    private string? selectedCountry;
    private EventViewModel selectedEvent = new();
    private string? selectedRegion;

    [ObservableProperty]
    private AddressViewModel userLocation = new();

    private IEnumerable<Event> RawEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public ObservableCollection<string> CountryCollection { get; set; } = [];
    public ObservableCollection<string> RegionCollection { get; set; } = [];
    public ObservableCollection<string> CityCollection { get; set; } = [];

    public ObservableCollection<string> UpcomingDateRanges { get; set; } = [];

    public ObservableCollection<string> CompletedDateRanges { get; set; } = [];

    [ObservableProperty]
    private bool isMapSelected;

    [ObservableProperty]
    private bool isListSelected;

    [ObservableProperty]
    private bool isUpcomingSelected;

    [ObservableProperty]
    private bool isCompletedSelected;

    [ObservableProperty]
    private bool areEventsFound;

    [ObservableProperty]
    private bool areNoEventsFound;

    private string selectedUpcomingDateRange = DateRanges.Today;

    private string selectedCompletedDateRange = DateRanges.Yesterday;

    public string SelectedUpcomingDateRange
    {
        get => selectedUpcomingDateRange;
        set
        {
            if (value == null)
            {
                return;
            }

            if (selectedUpcomingDateRange != value)
            {
                selectedUpcomingDateRange = value;
                OnPropertyChanged();
                HandleUpcomingDateRangeSelected();
            }
        }
    }

    public string SelectedCompletedDateRange
    {
        get => selectedCompletedDateRange;
        set
        {
            if (value == null)
            {
                return;
            }

            if (selectedCompletedDateRange != value)
            {
                selectedCompletedDateRange = value;
                OnPropertyChanged();
                HandleCompletedDateRangeSelected();
            }
        }
    }

    public string? SelectedCountry
    {
        get => selectedCountry;
        set
        {
            selectedCountry = value;
            OnPropertyChanged();

            HandleCountrySelected(value);
        }
    }

    public string? SelectedRegion
    {
        get => selectedRegion;
        set
        {
            selectedRegion = value;
            OnPropertyChanged();

            HandleRegionSelected(value);
        }
    }

    public string? SelectedCity
    {
        get => selectedCity;
        set
        {
            selectedCity = value;
            OnPropertyChanged();

            HandleCitySelected(value);
        }
    }

    public EventViewModel SelectedEvent
    {
        get => selectedEvent;
        set
        {
            if (selectedEvent != value)
            {
                selectedEvent = value;
                OnPropertyChanged(nameof(selectedEvent));

                if (selectedEvent != null)
                {
                    PerformNavigation(selectedEvent);
                }
            }
        }
    }

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            IsMapSelected = true;
            IsListSelected = false;
            IsUpcomingSelected = true;
            IsCompletedSelected = false;
            UserLocation = userManager.CurrentUser.GetAddress();

            foreach (var date in DateRanges.UpcomingRangeDictionary)
            {
                UpcomingDateRanges.Add(date.Key);
            }

            SelectedUpcomingDateRange = DateRanges.ThisMonth;

            foreach (var date in DateRanges.CompletedRangeDictionary)
            {
                CompletedDateRanges.Add(date.Key);
            }

            SelectedCompletedDateRange = DateRanges.LastMonth;

            IsBusy = false;

            await NotificationService.Notify("Event list has been refreshed.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while loading the events. Please try again in a few moments.");
        }
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task RefreshEvents()
    {
        Events.Clear();
        AreEventsFound = false;
        AreNoEventsFound = true;

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

        locations = await mobEventManager.GetLocationsByTimeRangeAsync(startDate, endDate);
        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        if (locations == null || !locations.Any())
        {
            return;
        }

        var countries = locations.Select(l => l.Country).Distinct();

        foreach (var country in countries)
        {
            if (!string.IsNullOrEmpty(country))
            {
                CountryCollection.Add(country);
            }
        }

        var eventFilter = new EventFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            PageIndex = 0,
            PageSize = 100,
            EventStatusId = null,
        };

        var events = await mobEventManager.GetFilteredEventsAsync(eventFilter);

        if (IsUpcomingSelected)
        {
            RawEvents = events.Where(e => !e.IsCompleted());
        }
        else
        {
            RawEvents = events.Where(e => e.IsCompleted());
        }

        if (!RawEvents.Any())
        {
            return;
        }

        var countryList = RawEvents.Select(e => e.Country).Distinct();

        UpdateEventReportViewModels();
    }

    private async void HandleUpcomingDateRangeSelected()
    {
        IsBusy = true;

        if (IsUpcomingSelected)
        {
            await RefreshEvents();
        }

        IsBusy = false;
    }

    private async void HandleCompletedDateRangeSelected()
    {
        IsBusy = true;

        if (IsCompletedSelected)
        {
            await RefreshEvents();
        }

        IsBusy = false;
    }

    private void HandleCountrySelected(string? selectedCountry)
    {
        IsBusy = true;

        if (selectedCountry != null)
        {
            RawEvents = RawEvents.Where(l => l.Country == SelectedCountry);
        }

        UpdateEventReportViewModels();

        RefreshRegionList();

        IsBusy = false;
    }

    private void RefreshRegionList()
    {
        RegionCollection.Clear();

        if (!locations.Any(l => l.Country == selectedCountry))
        {
            return;
        }

        var regions = locations.Where(l => l.Country == selectedCountry).Select(l => l.Region).Distinct();

        foreach (var region in regions)
        {
            if (!string.IsNullOrEmpty(region))
            {
                RegionCollection.Add(region);
            }
        }
    }

    private void HandleRegionSelected(string? selectedRegion)
    {
        IsBusy = true;

        if (!string.IsNullOrEmpty(selectedRegion))
        {
            RawEvents = RawEvents.Where(l => l.Region == selectedRegion);
        }

        UpdateEventReportViewModels();

        RefreshCityList();

        IsBusy = false;
    }

    private void RefreshCityList()
    {
        CityCollection.Clear();

        if (!locations.Any(l => l.Country == selectedCountry && l.Region == selectedRegion))
        {
            return;
        }

        var cities = locations.Where(l => l.Country == selectedCountry && l.Region == selectedRegion)
            .Select(l => l.City).Distinct();

        foreach (var city in cities)
        {
            if (!string.IsNullOrEmpty(city))
            {
                CityCollection.Add(city);
            }
        }
    }

    private void HandleCitySelected(string? selectedCity)
    {
        IsBusy = true;

        if (!string.IsNullOrEmpty(selectedCity))
        {
            RawEvents = RawEvents.Where(l => l.City == selectedCity);
        }

        UpdateEventReportViewModels();

        IsBusy = false;
    }

    private void UpdateEventReportViewModels()
    {
        Events.Clear();

        foreach (var mobEvent in RawEvents.OrderBy(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);
            
            Events.Add(vm);
        }

        AreEventsFound = Events.Any();
        AreNoEventsFound = !Events.Any();
    }

    [RelayCommand]
    private async Task ViewUpcoming()
    {
        IsBusy = true;

        IsUpcomingSelected = true;
        IsCompletedSelected = false;

        await RefreshEvents();

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ViewCompleted()
    {
        IsBusy = true;

        IsUpcomingSelected = false;
        IsCompletedSelected = true;

        await RefreshEvents();

        IsBusy = false;
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
    private async Task ClearSelections()
    {
        IsBusy = true;

        SelectedCountry = null;
        SelectedRegion = null;
        SelectedCity = null;

        await RefreshEvents();

        IsBusy = false;
    }
}