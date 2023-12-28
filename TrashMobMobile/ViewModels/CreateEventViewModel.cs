namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class CreateEventViewModel : BaseViewModel
{
    [ObservableProperty]
    EventViewModel eventViewModel;

    private readonly IMobEventManager mobEventManager;
    private readonly IEventTypeRestService eventTypeRestService;

    public CreateEventViewModel(IMobEventManager mobEventManager, IEventTypeRestService eventTypeRestService)
    {
        SaveEventCommand = new Command(async () => await SaveEvent());
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
    }

    public async Task Init()
    {
        EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
        
        foreach ( var eventType in EventTypes )
        {
            ETypes.Add(eventType.Name);
        }
    }

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    private List<EventType> EventTypes { get; set; } = [];

    public ObservableCollection<string> ETypes { get; set; }

    [ObservableProperty]
    string selectedEventType;

    public ICommand SaveEventCommand { get; set; }

    private async Task SaveEvent()
    {
        if ( !string.IsNullOrEmpty(SelectedEventType))
        {
            var eventType = EventTypes.FirstOrDefault(e => e.Name == SelectedEventType);
            if ( eventType != null )
            {
                EventViewModel.EventTypeId = eventType.Id;
            }
        }

        var mobEvent = EventViewModel.ToEvent();

        var updatedEvent = await mobEventManager.AddEventAsync(mobEvent);

        EventViewModel = updatedEvent.ToEventViewModel();
        Events.Clear();
        Events.Add(EventViewModel);

        await Notify("Event has been saved.");
    }
}
