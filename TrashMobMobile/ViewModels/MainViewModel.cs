#nullable enable

namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TrashMob.Models;
using TrashMobMobile.Authentication;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IUserRestService userRestService;
    private readonly IStatsRestService statsRestService;
    private readonly IMobEventManager mobEventManager;

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
    StatisticsViewModel statisticsViewModel = new StatisticsViewModel();

    private EventViewModel selectedEvent;
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
            WelcomeMessage = $"Welcome, {user.UserName}!";

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

        RefreshStatistics();
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

    [ObservableProperty]
    private string? welcomeMessage;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];
}
