namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class EditEventViewModel : BaseViewModel
{
    private readonly IEventTypeRestService eventTypeRestService;
    private readonly IMapRestService mapRestService;
    private readonly IMobEventManager mobEventManager;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    [ObservableProperty]
    private string selectedEventType;

    [ObservableProperty]
    private AddressViewModel userLocation;

    public EditEventViewModel(IMobEventManager mobEventManager,
        IEventTypeRestService eventTypeRestService,
        IMapRestService mapRestService)
    {
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
        this.mapRestService = mapRestService;
    }

    private Event MobEvent { get; set; }

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = new();

    private List<EventType> EventTypes { get; set; } = new();

    public ObservableCollection<string> ETypes { get; set; } = new();

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        UserLocation = App.CurrentUser.GetAddress();
        EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();

        MobEvent = await mobEventManager.GetEventAsync(eventId);

        SelectedEventType = EventTypes.First(et => et.Id == MobEvent.EventTypeId).Name;

        EventViewModel = MobEvent.ToEventViewModel();

        Events.Add(EventViewModel);

        foreach (var eventType in EventTypes)
        {
            ETypes.Add(eventType.Name);
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task SaveEvent()
    {
        IsBusy = true;
        if (!await Validate())
        {
            IsBusy = false;
            return;
        }

        if (!string.IsNullOrEmpty(SelectedEventType))
        {
            var eventType = EventTypes.FirstOrDefault(e => e.Name == SelectedEventType);
            if (eventType != null)
            {
                EventViewModel.EventTypeId = eventType.Id;
            }
        }

        // We need to copy back the property values that could have changed back to the event to be updated
        // (there are other values that cannot be updated) via the form that must be preserved on edit.
        MobEvent.City = EventViewModel.Address.City;
        MobEvent.Country = EventViewModel.Address.Country;
        MobEvent.Description = EventViewModel.Description;
        MobEvent.DurationHours = EventViewModel.DurationHours;
        MobEvent.DurationMinutes = EventViewModel.DurationMinutes;
        MobEvent.EventDate = EventViewModel.EventDate;
        MobEvent.EventTypeId = EventViewModel.EventTypeId;
        MobEvent.IsEventPublic = EventViewModel.IsEventPublic;
        MobEvent.Latitude = EventViewModel.Address.Latitude;
        MobEvent.Longitude = EventViewModel.Address.Longitude;
        MobEvent.MaxNumberOfParticipants = EventViewModel.MaxNumberOfParticipants;
        MobEvent.Name = EventViewModel.Name;
        MobEvent.PostalCode = EventViewModel.Address.PostalCode;
        MobEvent.Region = EventViewModel.Address.Region;
        MobEvent.StreetAddress = EventViewModel.Address.StreetAddress;

        MobEvent = await mobEventManager.UpdateEventAsync(MobEvent);

        EventViewModel = MobEvent.ToEventViewModel();
        Events.Clear();
        Events.Add(EventViewModel);

        IsBusy = false;

        await Notify("Event has been saved.");
    }

    public async Task ChangeLocation(Location location)
    {
        IsBusy = true;

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

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ManageEventPartners()
    {
        await Shell.Current.GoToAsync($"{nameof(ManageEventPartnersPage)}?EventId={EventViewModel.Id}");
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