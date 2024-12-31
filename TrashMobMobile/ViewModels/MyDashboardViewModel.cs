namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class MyDashboardViewModel(IMobEventManager mobEventManager,
                                          IStatsRestService statsRestService,
                                          ILitterReportManager litterReportManager,
                                          INotificationService notificationService,
                                          IUserManager userManager)
    : BaseViewModel(notificationService)
{
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IStatsRestService statsRestService = statsRestService;
    private EventViewModel? completedSelectedEvent;
    private LitterReportViewModel? selectedLitterReport;

    [ObservableProperty]
    public StatisticsViewModel statisticsViewModel = new StatisticsViewModel();

    private EventViewModel? upcomingSelectedEvent;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> CompletedEvents { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    public ObservableCollection<string> UpcomingDateRanges { get; set; } = [];

    public ObservableCollection<string> CompletedDateRanges { get; set; } = [];

    public ObservableCollection<string> CreatedDateRanges { get; set; } = [];

    [ObservableProperty]
    private bool areUpcomingEventsFound;

    [ObservableProperty]
    private bool areNoUpcomingEventsFound;

    [ObservableProperty]
    private bool areCompletedEventsFound;

    [ObservableProperty]
    private bool areNoCompletedEventsFound;

    [ObservableProperty]
    private bool areLitterReportsFound;

    [ObservableProperty]
    private bool areNoLitterReportsFound;

    private string selectedUpcomingDateRange = DateRanges.Today;

    private string selectedCompletedDateRange = DateRanges.Yesterday;

    private string selectedCreatedDateRange = DateRanges.LastWeek;

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

    public string SelectedCreatedDateRange
    {
        get => selectedCreatedDateRange;
        set
        {
            if (value == null)
            {
                return;
            }

            if (selectedCreatedDateRange != value)
            {
                selectedCreatedDateRange = value;
                OnPropertyChanged();
                HandleCreatedDateRangeSelected();
            }
        }
    }

    public EventViewModel? UpcomingSelectedEvent
    {
        get => upcomingSelectedEvent;
        set
        {
            if (upcomingSelectedEvent != value)
            {
                upcomingSelectedEvent = value;
                OnPropertyChanged();

                if (upcomingSelectedEvent != null)
                {
                    PerformEventNavigation(upcomingSelectedEvent);
                }
            }
        }
    }

    public EventViewModel? CompletedSelectedEvent
    {
        get => completedSelectedEvent;
        set
        {
            if (completedSelectedEvent != value)
            {
                completedSelectedEvent = value;
                OnPropertyChanged();

                if (completedSelectedEvent != null)
                {
                    PerformEventNavigation(completedSelectedEvent);
                }
            }
        }
    }

    public LitterReportViewModel? SelectedLitterReport
    {
        get => selectedLitterReport;
        set
        {
            if (selectedLitterReport != value)
            {
                selectedLitterReport = value;
                OnPropertyChanged();

                if (SelectedLitterReport != null)
                {
                    PerformLitterReportNavigation(SelectedLitterReport);
                }
            }
        }
    }

    public async Task Init()
    {
        IsBusy = true;

        try
        {
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

            foreach (var date in DateRanges.CreatedDateRangeDictionary)
            {
                CreatedDateRanges.Add(date.Key);
            }

            SelectedCreatedDateRange = DateRanges.LastMonth;

            var task1 = RefreshEvents();
            var task2 = RefreshStatistics();
            var task3 = RefreshLitterReports();

            await Task.WhenAll(task1, task2, task3);

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while loading the dashboard. Please wait and try again in a moment.");
        }
    }

    private async void PerformEventNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async void PerformLitterReportNavigation(LitterReportViewModel litterReportViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportViewModel.Id}");
    }

    private async Task RefreshStatistics()
    {
        var stats = await statsRestService.GetUserStatsAsync(userManager.CurrentUser.Id);

        StatisticsViewModel = new StatisticsViewModel
        {
            TotalBags = stats.TotalBags,
            TotalEvents = stats.TotalEvents,
            TotalHours = stats.TotalHours,
            TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted,
            TotalLitterReportsClosed = stats.TotalLitterReportsClosed,
        };
    }

    private async Task RefreshEvents()
    {
        CompletedEvents.Clear();
        UpcomingEvents.Clear();

        var upcomingStartDate = DateTimeOffset.Now.Date.AddDays(DateRanges.UpcomingRangeDictionary[SelectedUpcomingDateRange].Item1);
        var upcomingEndDate = DateTimeOffset.Now.Date.AddDays(DateRanges.UpcomingRangeDictionary[SelectedUpcomingDateRange].Item2);

        var upcomingEventFilter = new EventFilter
        {
            StartDate = upcomingStartDate,
            EndDate = upcomingEndDate,
        };

        var completedStartDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CompletedRangeDictionary[SelectedCompletedDateRange].Item1);
        var completedEndDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CompletedRangeDictionary[SelectedCompletedDateRange].Item2);

        var completedEventFilter = new EventFilter
        {
            StartDate = completedStartDate,
            EndDate = completedEndDate,
        };

        var upcomingEvents = await mobEventManager.GetUserEventsAsync(upcomingEventFilter, userManager.CurrentUser.Id);

        foreach (var mobEvent in upcomingEvents.OrderByDescending(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);
            vm.IsUserAttending = true;

            vm.CanCancelEvent = mobEvent.IsCancellable() && mobEvent.IsEventLead(userManager.CurrentUser.Id);
            UpcomingEvents.Add(vm);
        }

        var completedEvents = await mobEventManager.GetUserEventsAsync(completedEventFilter, userManager.CurrentUser.Id);

        foreach (var mobEvent in completedEvents.OrderByDescending(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);
            vm.IsUserAttending = true;
            vm.CanCancelEvent = false;
            CompletedEvents.Add(vm);
        }

        AreUpcomingEventsFound = UpcomingEvents.Any();
        AreNoUpcomingEventsFound = !UpcomingEvents.Any();
        AreCompletedEventsFound = CompletedEvents.Any();
        AreNoCompletedEventsFound = !CompletedEvents.Any();
    }

    private async Task RefreshLitterReports()
    {
        LitterReports.Clear();

        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item1);
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item2);

        var litterReportFilter = new LitterReportFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            CreatedByUserId = userManager.CurrentUser.Id,
        };

        var litterReports = await litterReportManager.GetLitterReportsAsync(litterReportFilter, TrashMob.Models.ImageSizeEnum.Thumb, true);

        foreach (var litterReport in litterReports.OrderByDescending(l => l.CreatedDate))
        {
            var vm = litterReport.ToLitterReportViewModel(NotificationService);
            LitterReports.Add(vm);
        }

        AreLitterReportsFound = LitterReports.Any();
        AreNoLitterReportsFound = !LitterReports.Any();
    }

    [RelayCommand]
    private async Task CreateLitterReport()
    {
        await Shell.Current.GoToAsync(nameof(CreateLitterReportPage));
    }

    [RelayCommand]
    private async Task CreateEvent()
    {
        await Shell.Current.GoToAsync($"{nameof(CreateEventPage)}?LitterReportId={Guid.Empty}");
    }

    private async void HandleUpcomingDateRangeSelected()
    {
        IsBusy = true;

        await RefreshEvents();

        IsBusy = false;
    }

    private async void HandleCompletedDateRangeSelected()
    {
        IsBusy = true;

        await RefreshEvents();

        IsBusy = false;
    }

    private async void HandleCreatedDateRangeSelected()
    {
        IsBusy = true;

        await RefreshLitterReports();

        IsBusy = false;
    }
}