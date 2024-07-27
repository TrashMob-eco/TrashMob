namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Authentication;
using TrashMobMobile.Config;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class MainViewModel(IAuthService authService,
    IUserRestService userRestService,
    IStatsRestService statsRestService,
    IMobEventManager mobEventManager,
    IToastService toastService) : BaseViewModel
{
    private readonly IAuthService authService = authService;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IStatsRestService statsRestService = statsRestService;
    private readonly IUserRestService userRestService = userRestService;
    private readonly IToastService toastService = toastService;
    private EventViewModel selectedEvent;

    [ObservableProperty]
    private StatisticsViewModel statisticsViewModel = new();

    [ObservableProperty]
    private int travelDistance;

    [ObservableProperty]
    private AddressViewModel userLocation;

    [ObservableProperty]
    private string userLocationDisplay = "Set Your Location Preference";

    [ObservableProperty]
    private string? welcomeMessage;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public EventViewModel SelectedEvent
    {
        get => selectedEvent;
        set
        {
            if (selectedEvent != value)
            {
                selectedEvent = value;
                OnPropertyChanged();

                if (selectedEvent != null)
                {
                    PerformNavigation(selectedEvent);
                }
            }
        }
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            var signedIn = await authService.SignInSilentAsync();

            if (signedIn.Succeeded)
            {
                var email = authService.GetUserEmail();
                var user = await userRestService.GetUserByEmailAsync(email, UserState.UserContext);

                WelcomeMessage = $"Welcome, {user.UserName}!";

                if (user.Latitude is not null && user.Longitude is not null)
                {
                    TravelDistance = user.TravelLimitForLocalEvents;
                    UserLocation = user.GetAddress();
                }
                else
                {
                    TravelDistance = Settings.DefaultTravelDistance;
                    UserLocation = GetDefaultAddress();
                }

                UserLocationDisplay = $"{UserLocation.City}, {UserLocation.Region}";

                await RefreshEvents();

                IsBusy = false;
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");
            }

            await RefreshStatistics();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError($"An error has occured while initializing the application. Please wait and try again in a moment.");
        }
    }

    private async Task RefreshStatistics()
    {
        var stats = await statsRestService.GetStatsAsync();

        StatisticsViewModel.TotalAttendees = stats.TotalParticipants;
        StatisticsViewModel.TotalBags = stats.TotalBags;
        StatisticsViewModel.TotalEvents = stats.TotalEvents;
        StatisticsViewModel.TotalHours = stats.TotalHours;
    }

    private async Task RefreshEvents()
    {
        UpcomingEvents.Clear();
        var events = await mobEventManager.GetActiveEventsAsync();

        var eventsUserIsAttending = await mobEventManager.GetEventsUserIsAttending(App.CurrentUser.Id);

        foreach (var mobEvent in events.OrderBy(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel();

            vm.IsUserAttending = eventsUserIsAttending.Any(e => e.Id == mobEvent.Id);

            UpcomingEvents.Add(vm);
        }
    }

    [RelayCommand]
    private async Task MyDashboard()
    {
        await Shell.Current.GoToAsync(nameof(MyDashboardPage));
    }

    [RelayCommand]
    private async Task CreateLitterReport()
    {
        await Shell.Current.GoToAsync(nameof(CreateLitterReportPage));
    }

    [RelayCommand]
    private async Task SearchLitterReports()
    {
        await Shell.Current.GoToAsync(nameof(SearchLitterReportsPage));
    }

    [RelayCommand]
    private async Task CreateEvent()
    {
#if USETEST
        await Shell.Current.GoToAsync(nameof(CreateEventPageNew));
#else
        await Shell.Current.GoToAsync(nameof(CreateEventPage));        
#endif
    }

    [RelayCommand]
    private async Task SetLocationPreference()
    {
        await Shell.Current.GoToAsync(nameof(SetUserLocationPreferencePage));
    }

    [RelayCommand]
    private async Task SearchEvents()
    {
        await Shell.Current.GoToAsync(nameof(SearchEventsPage));
    }

    private static AddressViewModel GetDefaultAddress()
    {
        return new AddressViewModel
        {
            City = Settings.DefaultCity,
            Region = Settings.DefaultRegion,
            Country = Settings.DefaultCountry,
            Location = new Location(Settings.DefaultLatitude, Settings.DefaultLongitude),
        };
    }
}