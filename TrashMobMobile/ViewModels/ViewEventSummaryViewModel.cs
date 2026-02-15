namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using Sentry;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewEventSummaryViewModel(IMobEventManager mobEventManager, 
                                               IPickupLocationManager pickupLocationManager,
                                               IEventAttendeeRouteRestService eventAttendeeRouteRestService,
                                               INotificationService notificationService,
                                               IUserManager userManager) 
    : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IPickupLocationManager pickupLocationManager = pickupLocationManager;
    private readonly IEventAttendeeRouteRestService eventAttendeeRouteRestService = eventAttendeeRouteRestService;
    private readonly IUserManager userManager = userManager;
    [ObservableProperty]
    private bool enableAddPickupLocation;

    [ObservableProperty]
    private bool enableEditEventSummary;

    [ObservableProperty]
    private EventSummaryViewModel eventSummaryViewModel = new();

    [ObservableProperty]
    private EventViewModel eventViewModel = new();

    [ObservableProperty]
    private bool isMapSelected;

    [ObservableProperty]
    private bool isListSelected;

    private PickupLocationViewModel? selectedPickupLocationViewModel;

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = [];

    public ObservableCollection<DisplayEventAttendeeRoute> EventAttendeeRoutes { get; set; } = [];

    private Action UpdateRoutes = null!;

    public PickupLocationViewModel? SelectedPickupLocation
    {
        get => selectedPickupLocationViewModel;
        set
        {
            if (value == selectedPickupLocationViewModel) return;
            selectedPickupLocationViewModel = value;
            OnPropertyChanged();

            if (selectedPickupLocationViewModel != null)
            {
                PerformNavigation(selectedPickupLocationViewModel.PickupLocation.Id);
            }
        }
    }

    private async void PerformNavigation(Guid pickupLocationId)
    {
        try
        {
            await Shell.Current.GoToAsync($"{nameof(ViewPickupLocationPage)}?PickupLocationId={pickupLocationId}");
            selectedPickupLocationViewModel = null;
            OnPropertyChanged(nameof(SelectedPickupLocation));
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    public async Task Init(Guid eventId, Action updRoutes)
    {
        UpdateRoutes = updRoutes;

        await ExecuteAsync(async () =>
        {
            IsMapSelected = true;
            IsListSelected = false;

            var mobEvent = await mobEventManager.GetEventAsync(eventId);
            EventViewModel = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);

            var eventSummary = await mobEventManager.GetEventSummaryAsync(eventId);

            if (eventSummary != null)
            {
                EventSummaryViewModel = new EventSummaryViewModel
                {
                    ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees,
                    DurationInMinutes = eventSummary.DurationInMinutes,
                    EventId = eventId,
                    Notes = eventSummary.Notes,
                    NumberOfBags = eventSummary.NumberOfBags,
                    PickedWeight = eventSummary.PickedWeight,
                    PickedWeightUnitId = eventSummary.PickedWeightUnitId,
                };
            }

            EnableEditEventSummary = mobEvent.IsEventLead(userManager.CurrentUser.Id);
            EnableAddPickupLocation = mobEvent.IsEventLead(userManager.CurrentUser.Id);

            var pickupLocations = await pickupLocationManager.GetPickupLocationsAsync(eventId, ImageSizeEnum.Thumb);

            PickupLocations.Clear();
            foreach (var pickupLocation in pickupLocations)
            {
                var pickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager, NotificationService, userManager)
                {
                    Address = new AddressViewModel
                    {
                        City = pickupLocation.City,
                        Country = pickupLocation.Country,
                        County = pickupLocation.County,
                        Location = new Microsoft.Maui.Devices.Sensors.Location(pickupLocation.Latitude ?? 0, pickupLocation.Longitude ?? 0),
                        Latitude = pickupLocation.Latitude ?? 0,
                        Longitude = pickupLocation.Longitude ?? 0,
                        PostalCode = pickupLocation.PostalCode,
                        Region = pickupLocation.Region,
                        StreetAddress = pickupLocation.StreetAddress,
                    },
                    Id = pickupLocation.Id,
                    Notes = pickupLocation.Notes,
                    Name = pickupLocation.Name,
                    Navigation = Navigation,
                    PickupLocation = pickupLocation,
                    ImageUrl = pickupLocation.ImageUrl,
                };

                await pickupLocationViewModel.Init(eventId);

                PickupLocations.Add(pickupLocationViewModel);
            }

            var routes = await eventAttendeeRouteRestService.GetEventAttendeeRoutesForEventAsync(eventId);
            EventAttendeeRoutes.Clear();

            foreach (var eventAttendeeRoute in routes)
            {
                EventAttendeeRoutes.Add(eventAttendeeRoute);
            }

            UpdateRoutes();
        }, "An error occurred while loading the event summary. Please try again.");
    }

    [RelayCommand]
    private async Task AddPickupLocation()
    {
        await Shell.Current.GoToAsync($"{nameof(CreatePickupLocationPage)}?EventId={EventSummaryViewModel.EventId}");
    }

    [RelayCommand]
    private async Task EditEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventSummaryPage)}?EventId={EventSummaryViewModel.EventId}");
    }

    [RelayCommand]
    private Task MapSelected()
    {
        IsMapSelected = true;
        IsListSelected = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ListSelected()
    {
        IsMapSelected = false;
        IsListSelected = true;
        return Task.CompletedTask;
    }
}