namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewEventSummaryViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private readonly IPickupLocationManager pickupLocationManager;

    public ViewEventSummaryViewModel(IMobEventManager mobEventManager, IPickupLocationManager pickupLocationManager)
    {
        EditEventSummaryCommand = new Command(async () => await EditEventSummary());
        AddPickupLocationCommand = new Command(async () => await AddPickupLocation());
        this.mobEventManager = mobEventManager;
        this.pickupLocationManager = pickupLocationManager;
    }

    [ObservableProperty]
    EventSummaryViewModel eventSummaryViewModel;

    [ObservableProperty]
    EventViewModel eventViewModel;

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new ObservableCollection<PickupLocationViewModel>();

    [ObservableProperty]
    bool enableEditEventSummary;

    [ObservableProperty]
    bool enableAddPickupLocation;

    public ICommand EditEventSummaryCommand { get; set; }
    public ICommand AddPickupLocationCommand { get; set; }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

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

        var pickupLocations = await pickupLocationManager.GetPickupLocationsAsync(eventId);

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
                PickupLocation = pickupLocation
            };

            await pickupLocationViewModel.Init(eventId);

            PickupLocations.Add(pickupLocationViewModel);
        }

        IsBusy = false;
    }

    private async Task AddPickupLocation()
    {
        await Shell.Current.GoToAsync($"{nameof(AddPickupLocationPage)}?EventId={EventSummaryViewModel.EventId}");
    }

    private async Task EditEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventSummaryPage)}?EventId={EventSummaryViewModel.EventId}");
    }
}
