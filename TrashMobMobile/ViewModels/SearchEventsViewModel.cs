namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class SearchEventsViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private EventViewModel selectedEvent;

    public SearchEventsViewModel(IMobEventManager mobEventManager)
    {
        this.mobEventManager = mobEventManager;
    }

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

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
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task RefreshEvents()
    {
        Events.Clear();
        var events = await mobEventManager.GetActiveEventsAsync();

        foreach (var mobEvent in events)
        {
            var vm = mobEvent.ToEventViewModel();
            Events.Add(vm);
        }

        await Notify("Event list has been refreshed.");
    }
}
