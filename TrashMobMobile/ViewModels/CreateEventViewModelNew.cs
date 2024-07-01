using System.Windows.Input;

namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class CreateEventViewModelNew : BaseViewModel
{
    private const int ActiveEventStatus = 1;
    private readonly IEventTypeRestService eventTypeRestService;
    private readonly IMapRestService mapRestService;

    private readonly IMobEventManager mobEventManager;
    private readonly IWaiverManager waiverManager;
    
    public ICommand PreviousCommand { get; set; }
    public ICommand NextCommand { get; set; }

    [ObservableProperty]
    private EventViewModel eventViewModel;

    [ObservableProperty]
    private bool isManageEventPartnersEnabled;

    [ObservableProperty]
    private string selectedEventType;

    [ObservableProperty]
    private AddressViewModel userLocation;

    public CreateEventViewModelNew(IMobEventManager mobEventManager,
        IEventTypeRestService eventTypeRestService,
        IMapRestService mapRestService,
        IWaiverManager waiverManager)
    {
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
        this.mapRestService = mapRestService;
        this.waiverManager = waiverManager;
    }

    public string DefaultEventName { get; } = "New Event";

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    private List<EventType> EventTypes { get; set; } = [];

    public ObservableCollection<string> ETypes { get; set; } = [];

    public async Task Init()
    {
        IsBusy = true;

        if (!await waiverManager.HasUserSignedTrashMobWaiverAsync())
        {
            await Shell.Current.GoToAsync($"{nameof(WaiverPage)}");
        }

        IsManageEventPartnersEnabled = false;

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
            EventStatusId = ActiveEventStatus,
        };

        SelectedEventType = EventTypes.OrderBy(e => e.DisplayOrder).First().Name;

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

        var mobEvent = EventViewModel.ToEvent();

        var updatedEvent = await mobEventManager.AddEventAsync(mobEvent);

        EventViewModel = updatedEvent.ToEventViewModel();
        Events.Clear();
        Events.Add(EventViewModel);

        IsManageEventPartnersEnabled = true;
        IsBusy = false;

        await Notify("Event has been saved.");
    }

    [RelayCommand]
    private async Task ManageEventPartners()
    {
        await Shell.Current.GoToAsync($"{nameof(ManageEventPartnersPage)}?EventId={EventViewModel.Id}");
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