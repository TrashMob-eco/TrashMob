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

    [ObservableProperty]
    AddressViewModel userLocation;

    private readonly IMobEventManager mobEventManager;
    private readonly IEventTypeRestService eventTypeRestService;
    private readonly IMapRestService mapRestService;
    private const int ActiveEventStatus = 1;

    public string DefaultEventName { get; } = "New Event";

    public CreateEventViewModel(IMobEventManager mobEventManager, 
                                IEventTypeRestService eventTypeRestService,
                                IMapRestService mapRestService)
    {
        SaveEventCommand = new Command(async () => await SaveEvent());
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
        this.mapRestService = mapRestService;
    }

    public async Task Init()
    {
        IsBusy = true;

        UserLocation = App.CurrentUser.GetAddress();
        EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();

        // Set defaults
        EventViewModel = new EventViewModel
        {
            Name = DefaultEventName,
            EventDate = DateTime.Now.AddDays(1),
            IsEventPublic = true,
            MaxNumberOfParticipants = 0,
            DurationHours = 2,
            DurationMinutes = 0,
            Address = UserLocation,
            EventTypeId = EventTypes.OrderBy(e => e.DisplayOrder).First().Id,
            EventStatusId = ActiveEventStatus
        };

        SelectedEventType = EventTypes.OrderBy(e => e.DisplayOrder).First().Name;

        Events.Add(EventViewModel);

        foreach ( var eventType in EventTypes )
        {
            ETypes.Add(eventType.Name);
        }

        IsBusy = false;
    }

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = new ObservableCollection<EventViewModel>();

    private List<EventType> EventTypes { get; set; } = new List<EventType>();

    public ObservableCollection<string> ETypes { get; set; } = new ObservableCollection<string>();

    [ObservableProperty]
    string selectedEventType;

    public ICommand SaveEventCommand { get; set; }

    private async Task SaveEvent()
    {
        IsBusy = true;

        if (!await Validate())
        {
            IsBusy = false;
            return;
        }

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

        IsBusy = false;

        await Notify("Event has been saved.");
    }

    public async Task ChangeLocation(Location location)
    {
        var addr = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);

        EventViewModel.Address.City = addr.City;
        EventViewModel.Address.Country = addr.Country;
        EventViewModel.Address.Latitude = location.Latitude;
        EventViewModel.Address.Longitude = location.Longitude;
        EventViewModel.Address.Location = location;
        EventViewModel.Address.PostalCode = addr.PostalCode;
        EventViewModel.Address.Region = addr.Region;
        EventViewModel.Address.StreetAddress = addr.StreetAddress;

        Events.Clear();
        Events.Add(EventViewModel);
    }

    private async Task<bool> Validate()
    {
        if (EventViewModel.IsEventPublic && EventViewModel.EventDate < DateTimeOffset.Now)
        {
            await NotifyError("Event Dates for new public events must be in the future.");
            return false;
        }

        return true;
    }
}
