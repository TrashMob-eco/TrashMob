namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Authentication;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IMobEventManager mobEventManager;
    private readonly IStatsRestService statsRestService;
    private readonly IUserRestService userRestService;
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

    public MainViewModel(IAuthService authService,
        IUserRestService userRestService,
        IStatsRestService statsRestService,
        IMobEventManager mobEventManager)
    {
        this.authService = authService;
        this.userRestService = userRestService;
        this.statsRestService = statsRestService;
        this.mobEventManager = mobEventManager;
    }

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

        var signedIn = await authService.SignInSilentAsync();

        if (signedIn.Succeeded)
        {
            var email = authService.GetUserEmail();
            var user = await userRestService.GetUserByEmailAsync(email, UserState.UserContext);
            if (user != null)
            {
                WelcomeMessage = $"Welcome, {user.UserName}!";
                UserLocation = App.CurrentUser.GetAddress();
                TravelDistance = App.CurrentUser.TravelLimitForLocalEvents;
                UserLocationDisplay = $"{UserLocation.City}, {UserLocation.Region}";
            }

            await RefreshEvents();

            IsBusy = false;
        }
        else
        {
            try
            {
                await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        await RefreshStatistics();
    }

    private async Task RefreshStatistics()
    {
        IsBusy = true;

        var stats = await statsRestService.GetStatsAsync();

        StatisticsViewModel.TotalAttendees = stats.TotalParticipants;
        StatisticsViewModel.TotalBags = stats.TotalBags;
        StatisticsViewModel.TotalEvents = stats.TotalEvents;
        StatisticsViewModel.TotalHours = stats.TotalHours;

        IsBusy = false;
    }

    private async Task RefreshEvents()
    {
        IsBusy = true;

        UpcomingEvents.Clear();
        var events = await mobEventManager.GetActiveEventsAsync();

        var eventsUserIsAttending = await mobEventManager.GetEventsUserIsAttending(App.CurrentUser.Id);

        foreach (var mobEvent in events.OrderBy(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel();

            vm.IsUserAttending = eventsUserIsAttending.Any(e => e.Id == mobEvent.Id);

            UpcomingEvents.Add(vm);
        }

        IsBusy = false;
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
        await Shell.Current.GoToAsync(nameof(CreateEventPage));
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
}