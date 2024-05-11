#nullable enable

namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TrashMobMobile.Authentication;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IUserRestService userRestService;
    private readonly IStatsRestService statsRestService;
    private readonly IMobEventManager mobEventManager;
    private EventViewModel selectedEvent;
    

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

    [ObservableProperty]
    private string? welcomeMessage;

    [ObservableProperty]
    string userLocationDisplay = "Set Your Location Preference";

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];


    [ObservableProperty]
    StatisticsViewModel statisticsViewModel = new StatisticsViewModel();

    [ObservableProperty]
    AddressViewModel userLocation;

    [ObservableProperty]
    int travelDistance;

    [ObservableProperty]
    double overlayOpacity;

    public EventViewModel SelectedEvent
    {
        get { return selectedEvent; }
        set
        {
            if (selectedEvent != value)
            {
                selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));

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
        OverlayOpacity = 0.25; // Workaround for: https://github.com/dotnet/maui/issues/18234

        var signedIn = await authService.SignInSilentAsync(true);

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
    private async Task ContactUs()
    {
        await Shell.Current.GoToAsync(nameof(ContactUsPage));
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

    [RelayCommand]
    private async Task Logout()
    {
        await authService.SignOutAsync();
        await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");
    }
}
