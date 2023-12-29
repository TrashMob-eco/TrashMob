namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MyDashboardViewModel : BaseViewModel
{
    private EventViewModel selectedEvent;
    private readonly IMobEventManager mobEventManager;

    public MyDashboardViewModel(IMobEventManager mobEventManager)
    {
        this.mobEventManager = mobEventManager;
    }

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];
    
    public ObservableCollection<EventViewModel> PastEvents { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

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
        await RefreshUpcomingEvents();
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task RefreshUpcomingEvents()
    {
        IsBusy = true;

        UpcomingEvents.Clear();
        var events = await mobEventManager.GetActiveEventsAsync();

        foreach (var mobEvent in events)
        {
            var vm = mobEvent.ToEventViewModel();
            UpcomingEvents.Add(vm);
        }

        IsBusy = false;

        await Notify("Upcoming event list has been refreshed.");
    }
}
