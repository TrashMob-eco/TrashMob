#nullable enable

namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
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

        ContactUsCommand = new Command(async () => await ContactUs());
        MyDashboardCommand = new Command(async () => await MyDashboard());
        SearchEventsCommand = new Command(async () => await SearchEvents());
        CreateEventCommand = new Command(async () => await CreateEvent());
        SubmitLitterReportCommand = new Command(async () => await SubmitLitterReport());
        SearchLitterReportsCommand = new Command(async () => await SearchLitterReports());
        SetLocationPreferenceCommand = new Command(async () => await SetLocationPreference());
        LogoutCommand = new Command(async () => await Logout());
    }

    [ObservableProperty]
    private string? welcomeMessage;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public ICommand ContactUsCommand { get; set; }
    public ICommand MyDashboardCommand { get; set; }
    public ICommand SearchEventsCommand { get; set; }
    public ICommand CreateEventCommand { get; set; }
    public ICommand SubmitLitterReportCommand { get; set; }
    public ICommand SearchLitterReportsCommand { get; set; }
    public ICommand SetLocationPreferenceCommand { get; set; }
    public ICommand LogoutCommand { get; set; }

    [ObservableProperty]
    StatisticsViewModel statisticsViewModel = new StatisticsViewModel();

    [ObservableProperty]
    AddressViewModel userLocation;

    [ObservableProperty]
    int travelDistance;

    public EventViewModel SelectedEvent
    {
        get { return selectedEvent; }
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

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Navigation.PushAsync(new ViewEventPage(new ViewEventViewModel { EventViewModel = eventViewModel }));
    }

    public async Task Init()
    {
        IsBusy = true;

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
            }

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
        await RefreshEvents();
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

        foreach (var mobEvent in events)
        {
            var vm = mobEvent.ToEventViewModel();
            UpcomingEvents.Add(vm);
        }
    }

    private async Task ContactUs()
    {
        await Shell.Current.GoToAsync(nameof(ContactUsPage));
    }

    private async Task MyDashboard()
    {
        await Shell.Current.GoToAsync(nameof(MyDashboardPage));
    }

    private async Task SubmitLitterReport()
    {
        await Shell.Current.GoToAsync(nameof(SubmitLitterReportPage));
    }

    private async Task SearchLitterReports()
    {
        await Shell.Current.GoToAsync(nameof(SearchLitterReportsPage));
    }

    private async Task CreateEvent()
    {
        await Shell.Current.GoToAsync(nameof(CreateEventPage));
    }

    private async Task SetLocationPreference()
    {
        await Shell.Current.GoToAsync(nameof(SetUserLocationPreferencePage));
    }

    private async Task SearchEvents()
    {
        await Shell.Current.GoToAsync(nameof(SearchEventsPage));
    }

    private async Task Logout()
    {
    }
}
