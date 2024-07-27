namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class MyDashboardViewModel(IMobEventManager mobEventManager, IStatsRestService statsRestService,
    ILitterReportManager litterReportManager) : BaseViewModel
{
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IStatsRestService statsRestService = statsRestService;
    private EventViewModel? completedSelectedEvent;
    private LitterReportViewModel? selectedLitterReport;

    [ObservableProperty]
    public StatisticsViewModel statisticsViewModel;

    private EventViewModel? upcomingSelectedEvent;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> CompletedEvents { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

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
            await NotifyError("An error has occurred while loading the dashboard. Please wait and try again in a moment.");
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
        var stats = await statsRestService.GetUserStatsAsync(App.CurrentUser.Id);

        StatisticsViewModel = new StatisticsViewModel
        {
            TotalBags = stats.TotalBags,
            TotalEvents = stats.TotalEvents,
            TotalHours = stats.TotalHours,
        };
    }

    private async Task RefreshEvents()
    {
        CompletedEvents.Clear();
        UpcomingEvents.Clear();

        var events = await mobEventManager.GetUserEventsAsync(App.CurrentUser.Id, false);

        foreach (var mobEvent in events.OrderByDescending(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel();
            vm.IsUserAttending = true;

            if (mobEvent.IsCompleted())
            {
                vm.CanCancelEvent = false;
                CompletedEvents.Add(vm);
            }
            else
            {
                vm.CanCancelEvent = mobEvent.IsCancellable() && mobEvent.IsEventLead();
                UpcomingEvents.Add(vm);
            }
        }
    }

    private async Task RefreshLitterReports()
    {
        LitterReports.Clear();

        var litterReports = await litterReportManager.GetUserLitterReportsAsync(App.CurrentUser.Id);

        foreach (var litterReport in litterReports.OrderByDescending(l => l.CreatedDate))
        {
            var vm = litterReport.ToLitterReportViewModel();
            LitterReports.Add(vm);
        }
    }

    [RelayCommand]
    private async Task CreateLitterReport()
    {
        await Shell.Current.GoToAsync(nameof(CreateLitterReportPage));
    }

    [RelayCommand]
    private async Task CreateEvent()
    {
        await Shell.Current.GoToAsync(nameof(CreateEventPage));
    }
}