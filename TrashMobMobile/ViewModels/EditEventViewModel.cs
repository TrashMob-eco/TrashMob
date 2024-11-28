namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class EditEventViewModel(IMobEventManager mobEventManager,
    IEventTypeRestService eventTypeRestService,
    IMapRestService mapRestService,
    INotificationService notificationService,
    IUserManager userManager)
    : BaseViewModel(notificationService)
{
    private readonly IEventTypeRestService eventTypeRestService = eventTypeRestService;
    private readonly IMapRestService mapRestService = mapRestService;
    private readonly IUserManager userManager = userManager;
    private readonly IMobEventManager mobEventManager = mobEventManager;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    [ObservableProperty]
    private string selectedEventType;

    [ObservableProperty]
    private AddressViewModel userLocation;

    private Event MobEvent { get; set; }

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = new();

    private List<EventType> EventTypes { get; set; } = new();

    public ObservableCollection<string> ETypes { get; set; } = new();

    [ObservableProperty]
    private bool isDetailsVisible;

    [ObservableProperty]
    private bool isLocationVisible;

    [ObservableProperty]
    private bool isPartnersVisible;

    [ObservableProperty]
    private bool isLitterReportsVisible;

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            UserLocation = userManager.CurrentUser.GetAddress();
            EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
            IsDetailsVisible = false;
            IsLocationVisible = true;
            IsPartnersVisible = false;
            IsLitterReportsVisible = false;

            MobEvent = await mobEventManager.GetEventAsync(eventId);

            SelectedEventType = EventTypes.First(et => et.Id == MobEvent.EventTypeId).Name;

            EventViewModel = MobEvent.ToEventViewModel(userManager.CurrentUser.Id);

            Events.Add(EventViewModel);

            foreach (var eventType in EventTypes)
            {
                ETypes.Add(eventType.Name);
            }

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while loading the event. Please wait and try again in a moment.");
        }
    }

    [RelayCommand]
    private async Task SaveEvent()
    {
        IsBusy = true;

        try
        {
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

            EventViewModel = MobEvent.ToEventViewModel(userManager.CurrentUser.Id);
            Events.Clear();
            Events.Add(EventViewModel);

            IsBusy = false;

            await NotificationService.Notify("Event has been saved.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while saving the event. Please wait and try again in a moment.");
        }
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
        IsDetailsVisible = false;
        IsLocationVisible = false;
        IsPartnersVisible = true;
        IsLitterReportsVisible = false;
    }

    [RelayCommand]
    private async Task ManageLitterReports()
    {
        IsDetailsVisible = false;
        IsLocationVisible = false;
        IsPartnersVisible = false;
        IsLitterReportsVisible = true;
    }

    [RelayCommand]
    private async Task ManageEventDetails()
    {
        IsDetailsVisible = true;
        IsLocationVisible = false;
        IsPartnersVisible = false;
        IsLitterReportsVisible = false;
    }

    [RelayCommand]
    private async Task ManageEventLocation()
    {
        IsDetailsVisible = false;
        IsLocationVisible = true;
        IsPartnersVisible = false;
        IsLitterReportsVisible = false;
    }

    private async Task<bool> Validate()
    {
        if (EventViewModel.IsEventPublic && EventViewModel.EventDate < DateTimeOffset.Now)
        {
            await NotificationService.NotifyError("Event Dates for new public events must be in the future.");
            return false;
        }

        return true;
    }
}