namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MyDashboardViewModel : BaseViewModel
{
    private EventViewModel selectedEvent;
    private readonly IMobEventManager mobEventManager;
    private readonly IStatsRestService statsRestService;

    public MyDashboardViewModel(IMobEventManager mobEventManager, IStatsRestService statsRestService)
    {
        this.mobEventManager = mobEventManager;
        this.statsRestService = statsRestService;
    }

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> CompletedEvents { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    [ObservableProperty]
    public StatisticsViewModel statisticsViewModel;

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

    public async Task Init()
    {
        await RefreshEvents();
        await RefreshStatistics();
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task RefreshStatistics()
    {
        IsBusy = true;

        var stats = await statsRestService.GetUserStatsAsync(App.CurrentUser.Id);

        StatisticsViewModel = new StatisticsViewModel
        {
            TotalBags = stats.TotalBags,
            TotalEvents = stats.TotalEvents,
            TotalHours = stats.TotalHours,
        };

        IsBusy = false;
    }

    private async Task RefreshEvents()
    {
        IsBusy = true;

        CompletedEvents.Clear();
        UpcomingEvents.Clear();

        var events = await mobEventManager.GetUserEventsAsync(App.CurrentUser.Id, false);

        foreach (var mobEvent in events.OrderByDescending(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel();
            vm.IsUserAttending = true;

            if (mobEvent.EventDate < DateTime.UtcNow)
            {
                CompletedEvents.Add(vm);
            }
            else
            {
                UpcomingEvents.Add(vm);
            }
        }

        IsBusy = false;

        await Notify("Event list has been refreshed.");
    }
}
