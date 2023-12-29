namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewEventViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private readonly IEventTypeRestService eventTypeRestService;

    public ViewEventViewModel(IMobEventManager mobEventManager, IEventTypeRestService eventTypeRestService)
    {
        RegisterCommand = new Command(async () => await Register());
        UnregisterCommand = new Command(async () => await Unregister());
        EditEventCommand = new Command(async () => await EditEvent());
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
    }

    [ObservableProperty]
    EventViewModel eventViewModel;

    [ObservableProperty]
    bool enableRegister;

    [ObservableProperty]
    bool enableUnregister;

    [ObservableProperty]
    bool enableEditEvent;

    [ObservableProperty]
    string selectedEventType;

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public ICommand RegisterCommand { get; set; }

    public ICommand UnregisterCommand { get; set; }

    public ICommand EditEventCommand { get; set; }

    public async Task Init(Guid eventId)
    {
        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();

        var eventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
        SelectedEventType = eventTypes.First(et => et.Id == mobEvent.EventTypeId).Name;
        
        Events.Clear();
        Events.Add(EventViewModel);
        var isAttending = await mobEventManager.IsUserAttendingAsync(eventId, App.CurrentUser.Id);

        EnableRegister = !mobEvent.IsEventLead() && !isAttending && mobEvent.AreNewRegistrationsAllowed();
        EnableUnregister = !mobEvent.IsEventLead() && isAttending && mobEvent.AreUnregistrationsAllowed();
        EnableEditEvent = mobEvent.IsEventLead();
    }

    private async Task EditEvent()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task Register()
    {
        var eventAttendee = new EventAttendee()
        {
            EventId = EventViewModel.Id,
            UserId = App.CurrentUser.Id
        };

        await mobEventManager.AddEventAttendeeAsync(eventAttendee);

        await Notify("You have been registered for this event.");
    }

    private async Task Unregister()
    {
        var eventAttendee = new EventAttendee()
        {
            EventId = EventViewModel.Id,
            UserId = App.CurrentUser.Id
        };

        await mobEventManager.RemoveEventAttendeeAsync(eventAttendee);

        await Notify("You have been unregistered for this event.");
    }
}
