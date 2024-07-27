namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewEventSummaryViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private readonly IPickupLocationManager pickupLocationManager;

    [ObservableProperty]
    private bool enableAddPickupLocation;

    [ObservableProperty]
    private bool enableEditEventSummary;

    [ObservableProperty]
    private EventSummaryViewModel eventSummaryViewModel;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    private PickupLocationViewModel selectedPickupLocationViewModel;

    public ViewEventSummaryViewModel(IMobEventManager mobEventManager, IPickupLocationManager pickupLocationManager)
    {
        this.mobEventManager = mobEventManager;
        this.pickupLocationManager = pickupLocationManager;
    }

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new();

    public PickupLocationViewModel SelectedPickupLocation
    {
        get => selectedPickupLocationViewModel;
        set
        {
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
        await Shell.Current.GoToAsync($"{nameof(ViewPickupLocationPage)}?PickupLocationId={pickupLocationId}");
    }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            var mobEvent = await mobEventManager.GetEventAsync(eventId);
            EventViewModel = mobEvent.ToEventViewModel();

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
                };
            }

            EnableEditEventSummary = mobEvent.IsEventLead();
            EnableAddPickupLocation = mobEvent.IsEventLead();

            var pickupLocations = await pickupLocationManager.GetPickupLocationsAsync(eventId, ImageSizeEnum.Thumb);

            PickupLocations.Clear();
            foreach (var pickupLocation in pickupLocations)
            {
                var pickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager)
                {
                    Address = new AddressViewModel
                    {
                        City = pickupLocation.City,
                        Country = pickupLocation.Country,
                        County = pickupLocation.County,
                        Location = new Location(pickupLocation.Latitude.Value, pickupLocation.Longitude.Value),
                        Latitude = pickupLocation.Latitude.Value,
                        Longitude = pickupLocation.Longitude.Value,
                        PostalCode = pickupLocation.PostalCode,
                        Region = pickupLocation.Region,
                        StreetAddress = pickupLocation.StreetAddress,
                    },
                    Id = pickupLocation.Id,
                    Notes = pickupLocation.Notes,
                    Name = pickupLocation.Name,
                    Notify = Notify,
                    NotifyError = NotifyError,
                    Navigation = Navigation,
                    PickupLocation = pickupLocation,
                    ImageUrl = pickupLocation.ImageUrl,
                };

                await pickupLocationViewModel.Init(eventId);

                PickupLocations.Add(pickupLocationViewModel);
            }

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error occured while loading the event summary. Please try again.");
        }
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
}