namespace TrashMobMobileApp.Features.Map;

using Microsoft.AspNetCore.Components;
using Maui.GoogleMaps;
using TrashMob.Models;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Extensions;
using TrashMobMobileApp.StateContainers;

public partial class MauiMapPageMultipleEvent : ContentPage
{
    private readonly IEnumerable<Event> mobEvents;
    private List<Guid> userAttendingEventIds = new();
    private User user;
    private Guid selectedEventId = Guid.Empty;

    [Inject]
    public IMobEventManager MobEventManager { get; set; }

    [Inject]
    public IWaiverManager WaiverManager { get; set; }

    [Inject]
    public UserStateInformation StateInformation { get; set; }

    public MauiMapPageMultipleEvent(IMobEventManager mobEventManager, IWaiverManager waiverManager, UserStateInformation stateInformation, IEnumerable<Event> mobEvents)
    {
        InitializeComponent();

        user = App.CurrentUser;

        mappy.MyLocationEnabled = true;

        foreach (var mobEvent in mobEvents)
        {
            var location = new Position(mobEvent.Latitude.Value, mobEvent.Longitude.Value);

            var pin = MapHelper.GetPinForEvent(mobEvent);
            pin.Position = location;
            mappy.Pins.Add(pin);
        }

        MobEventManager = mobEventManager;
        WaiverManager = waiverManager;
        StateInformation = stateInformation;
        this.mobEvents = mobEvents;
    }

    private void Mappy_PinClicked(object sender, PinClickedEventArgs e)
    {
        var pin = e.Pin as TrashMobPin;

        if (pin != null)
        {
            var selectedEvent = mobEvents.FirstOrDefault(x => x.Id == pin.EventId);

            SetFields(selectedEvent);
            selectedEventId = selectedEvent.Id;
            addressDisplay.IsVisible = true;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var locationHelper = new LocationHelper();
        var userLocation = await locationHelper.GetCurrentLocation();

        userAttendingEventIds = (await MobEventManager.GetEventsUserIsAttending(user.Id)).Select(x => x.Id).ToList();

        if (userLocation != null)
        {
            var mapSpan = new MapSpan(userLocation, LocationHelper.DefaultLatitudeDegreesMultipleEvents, LocationHelper.DefaultLongitudeDegreesMultipleEvents);
            mappy.MoveToRegion(mapSpan);
        }
        else
        {
            var defaultLocation = new Position(LocationHelper.DefaultLatitude, LocationHelper.DefaultLongitude);
            var mapSpan = new MapSpan(defaultLocation, LocationHelper.DefaultLatitudeDegreesMultipleEvents, LocationHelper.DefaultLongitudeDegreesMultipleEvents);
            mappy.MoveToRegion(mapSpan);
        }
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        var hasSignedWaiver = await WaiverManager.HasUserSignedTrashMobWaiverAsync();

        if (!hasSignedWaiver)
        {
            StateInformation.HasToSignWaiver = true;
            await Navigation.PopModalAsync();
        }

        var attendee = new EventAttendee
        {
            UserId = user.Id,
            EventId = selectedEventId
        };

        await MobEventManager.AddEventAttendeeAsync(attendee);

        userAttendingEventIds = (await MobEventManager.GetEventsUserIsAttending(user.Id)).Select(x => x.Id).ToList();

        var selectedEvent = mobEvents.FirstOrDefault(x => x.Id == selectedEventId);

        if (selectedEvent != null)
        {
            SetFields(selectedEvent);
        }
        else
        {
            selectedEventId = Guid.Empty;
            addressDisplay.IsVisible = true;
        }
    }

    private async void UnregisterButton_Clicked(object sender, EventArgs e)
    {
        var attendee = new EventAttendee
        {
            UserId = user.Id,
            EventId = selectedEventId
        };

        await MobEventManager.RemoveEventAttendeeAsync(attendee);

        userAttendingEventIds = (await MobEventManager.GetEventsUserIsAttending(user.Id)).Select(x => x.Id).ToList();
        
        var selectedEvent = mobEvents.FirstOrDefault(x => x.Id == selectedEventId);

        if (selectedEvent != null)
        {
            SetFields(selectedEvent);
        }
        else
        {
            selectedEventId = Guid.Empty;
            addressDisplay.IsVisible = true;
        }
    }

    private void mappy_MapClicked(object sender, MapClickedEventArgs e)
    {
        addressDisplay.IsVisible = false;
        selectedEventId = Guid.Empty;
    }

    private void SetFields(Event mobEvent)
    {
        eventName.Text = mobEvent.Name;
        eventDate.Text = mobEvent.EventDate.GetFormattedLocalDate();
        eventTime.Text = mobEvent.EventDate.GetFormattedLocalTime();
        streetAddress.Text = mobEvent.StreetAddress;
        city.Text = mobEvent.City;
        state.Text = mobEvent.Region;
        postalCode.Text = mobEvent.PostalCode;

        isEventLead.IsVisible = mobEvent.CreatedByUserId != user.Id;

        register.IsVisible = !userAttendingEventIds.Contains(mobEvent.Id);
        unregister.IsVisible = userAttendingEventIds.Contains(mobEvent.Id);
    }

    private void addressDisplay_Loaded(object sender, EventArgs e)
    {
        addressDisplay.IsVisible = false;
    }
}