﻿namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Authentication;
using TrashMobMobile.Config;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class MainViewModel(IAuthService authService,
    IUserRestService userRestService,
    IStatsRestService statsRestService,
    IMobEventManager mobEventManager,
    ILitterReportManager litterReportManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IAuthService authService = authService;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IStatsRestService statsRestService = statsRestService;
    private readonly IUserRestService userRestService = userRestService;
    private readonly INotificationService notificationService = notificationService;
    private readonly IUserManager userManager = userManager;

#nullable disable
    private EventViewModel selectedEvent;
#nullable restore

    [ObservableProperty]
    private StatisticsViewModel statisticsViewModel = new();

    [ObservableProperty]
    private int travelDistance;

#nullable disable
    [ObservableProperty]
    private AddressViewModel userLocation;
#nullable restore

    [ObservableProperty]
    private string userLocationDisplay = "Set Your Location Preference";

    [ObservableProperty]
    private string? welcomeMessage;

    [ObservableProperty]
    private bool isMapSelected;

    [ObservableProperty]
    private bool isListSelected;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];
    
    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

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

                Addresses.Clear();

                var tasks = new List<Task>
                {
                    RefreshEvents(),
                    RefreshLitterReports(),
                };

                await Task.WhenAll(tasks);

                IsMapSelected = true;
                IsListSelected = false;

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
            await NotificationService.NotifyError($"An error occurred while initializing the application. Please wait and try again in a moment.");
        }
    }

    private async Task RefreshStatistics()
    {
        var stats = await statsRestService.GetStatsAsync();

        StatisticsViewModel.TotalAttendees = stats.TotalParticipants;
        StatisticsViewModel.TotalBags = stats.TotalBags;
        StatisticsViewModel.TotalEvents = stats.TotalEvents;
        StatisticsViewModel.TotalHours = stats.TotalHours;
        StatisticsViewModel.TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted;
        StatisticsViewModel.TotalLitterReportsClosed = stats.TotalLitterReportsSubmitted;
    }

    private async Task RefreshLitterReports()
    {
        LitterReports.Clear();
        var litterFilter = new LitterReportFilter
        {
            LitterReportStatusId = (int)LitterReportStatusEnum.New,
            IncludeLitterImages = true,
        };

        var litterReports = await litterReportManager.GetLitterReportsAsync(litterFilter, ImageSizeEnum.Thumb, getImageUrls: false);

        foreach (var litterReport in litterReports.OrderBy(l => l.CreatedDate))
        {
            var vm = litterReport.ToLitterReportViewModel(notificationService);
            LitterReports.Add(vm);

            foreach (var litterImageViewModel in vm.LitterImageViewModels)
            {
                Addresses.Add(litterImageViewModel.Address);
            }
        }
    }

    private async Task RefreshEvents()
    {
        UpcomingEvents.Clear();
        var startDate = DateTimeOffset.Now;
        var endDate = DateTimeOffset.Now.AddDays(3650);

        var eventFilter = new EventFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            PageIndex = 0,
            PageSize = 50,
            EventStatusId = null,
        };

        var events = await mobEventManager.GetFilteredEventsAsync(eventFilter);

        var eventsUserIsAttending = await mobEventManager.GetEventsUserIsAttending(userManager.CurrentUser.Id);

        foreach (var mobEvent in events.OrderBy(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);

            vm.IsUserAttending = eventsUserIsAttending.Any(e => e.Id == mobEvent.Id);

            UpcomingEvents.Add(vm);
            Addresses.Add(vm.Address);
        }
    }

    [RelayCommand]
    private async Task MyDashboard()
    {
        await Shell.Current.GoToAsync(nameof(MyDashboardPage));
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
        await Shell.Current.GoToAsync($"{nameof(CreateEventPage)}?LitterReportId={Guid.Empty}");
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
            Location = new Microsoft.Maui.Devices.Sensors.Location(Settings.DefaultLatitude, Settings.DefaultLongitude),
        };
    }
}