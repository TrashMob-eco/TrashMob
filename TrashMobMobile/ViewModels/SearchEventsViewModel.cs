namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class SearchEventsViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    [ObservableProperty]
    private string eventStatus = "Upcoming";

    private string? selectedCity;
    private string? selectedCountry;
    private EventViewModel selectedEvent;
    private string? selectedRegion;

    [ObservableProperty]
    private AddressViewModel userLocation;

    public SearchEventsViewModel(IMobEventManager mobEventManager)
    {
        this.mobEventManager = mobEventManager;
    }

    private IEnumerable<Event> RawEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public ObservableCollection<string> CountryCollection { get; set; } = [];
    public ObservableCollection<string> RegionCollection { get; set; } = [];
    public ObservableCollection<string> CityCollection { get; set; } = [];

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
        UserLocation = App.CurrentUser.GetAddress();
        await RefreshEvents();
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task RefreshEvents()
    {
        IsBusy = true;

        Events.Clear();

        if (EventStatus == "Upcoming")
        {
            RawEvents = await mobEventManager.GetActiveEventsAsync();
        }
        else if (EventStatus == "Completed")
        {
            RawEvents = await mobEventManager.GetCompletedEventsAsync();
        }
        else
        {
            RawEvents = await mobEventManager.GetAllEventsAsync();
        }

        var countryList = RawEvents.Select(e => e.Country).Distinct();

        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        foreach (var country in countryList)
        {
            CountryCollection.Add(country);
        }

        UpdateEventReportViewModels();

        IsBusy = false;

        await Notify("Event list has been refreshed.");
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
        var regionList = RawEvents.Select(e => e.Region).Distinct();

        RegionCollection.Clear();

        foreach (var region in regionList)
        {
            RegionCollection.Add(region);
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
        var cityList = RawEvents.Select(e => e.City).Distinct();

        CityCollection.Clear();

        foreach (var city in cityList)
        {
            CityCollection.Add(city);
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

        foreach (var mobEvent in RawEvents)
        {
            var vm = mobEvent.ToEventViewModel();
            Events.Add(vm);
        }
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