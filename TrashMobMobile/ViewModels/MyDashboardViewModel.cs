namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MyDashboardViewModel : BaseViewModel
{
    private EventViewModel? upcomingSelectedEvent;
    private EventViewModel? completedSelectedEvent;
    private LitterReportViewModel? selectedLitterReport;
    private readonly IMobEventManager mobEventManager;
    private readonly IStatsRestService statsRestService;
    private readonly ILitterReportManager litterReportManager;

    public MyDashboardViewModel(IMobEventManager mobEventManager, IStatsRestService statsRestService, ILitterReportManager litterReportManager)
    {
        this.mobEventManager = mobEventManager;
        this.statsRestService = statsRestService;
        this.litterReportManager = litterReportManager;
    }

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> CompletedEvents { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    [ObservableProperty]
    public StatisticsViewModel statisticsViewModel;

    public EventViewModel? UpcomingSelectedEvent
    {
        get { return upcomingSelectedEvent; }
        set
        {
            if (upcomingSelectedEvent != value)
            {
                upcomingSelectedEvent = value;
                OnPropertyChanged(nameof(UpcomingSelectedEvent));

                if (upcomingSelectedEvent != null)
                {
                    PerformEventNavigation(upcomingSelectedEvent);
                }
            }
        }
    }

    public EventViewModel? CompletedSelectedEvent
    {
        get { return completedSelectedEvent; }
        set
        {
            if (completedSelectedEvent != value)
            {
                completedSelectedEvent = value;
                OnPropertyChanged(nameof(CompletedSelectedEvent));

                if (completedSelectedEvent != null)
                {
                    PerformEventNavigation(completedSelectedEvent);
                }
            }
        }
    }

    public LitterReportViewModel? SelectedLitterReport
    {
        get { return selectedLitterReport; }
        set
        {
            if (selectedLitterReport != value)
            {
                selectedLitterReport = value;
                OnPropertyChanged(nameof(SelectedLitterReport));

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
        
        var task1 = RefreshEvents();
        var task2 = RefreshStatistics();
        var task3 = RefreshLitterReports();

        await Task.WhenAll(task1, task2, task3);
        
        IsBusy = false;
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
}
