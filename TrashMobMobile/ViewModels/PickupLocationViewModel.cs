namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class PickupLocationViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    private readonly IPickupLocationManager pickupLocationManager;

    [ObservableProperty]
    private AddressViewModel address;

    [ObservableProperty]
    private bool canDeletePickupLocation;

    [ObservableProperty]
    private bool canEditPickupLocation;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string imageUrl;

    private Event mobEvent;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string notes;

    public PickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
        this.pickupLocationManager = pickupLocationManager;
        this.mobEventManager = mobEventManager;
    }

    public PickupLocation PickupLocation { get; set; }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;
        mobEvent = await mobEventManager.GetEventAsync(eventId);

        CanDeletePickupLocation = mobEvent.IsEventLead();
        CanEditPickupLocation = mobEvent.IsEventLead();

        IsBusy = false;
    }

    [RelayCommand]
    private async Task DeletePickupLocation()
    {
        await pickupLocationManager.DeletePickupLocationAsync(PickupLocation);

        await Notify("Pickup location has been removed.");

        await Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task EditPickupLocation()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(EditPickupLocationPage)}?EventId={mobEvent.Id}&PickupLocationId={PickupLocation.Id}");
    }
}