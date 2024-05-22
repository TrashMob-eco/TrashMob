namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class PickupLocationViewModel : BaseViewModel
{
    public PickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
        this.pickupLocationManager = pickupLocationManager;
        this.mobEventManager = mobEventManager;
    }

    public PickupLocation PickupLocation { get; set; }

    private Event mobEvent;

    public async Task Init(Guid eventId)
    {
        IsBusy = true;
        mobEvent = await mobEventManager.GetEventAsync(eventId);

        CanDeletePickupLocation = mobEvent.IsEventLead();
        CanEditPickupLocation = mobEvent.IsEventLead();

        IsBusy = false;
    }

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string notes;

    [ObservableProperty]
    AddressViewModel address;

    [ObservableProperty]
    bool canDeletePickupLocation;

    [ObservableProperty]
    bool canEditPickupLocation;

    [ObservableProperty]
    string imageUrl;

    private readonly IPickupLocationManager pickupLocationManager;
    private readonly IMobEventManager mobEventManager;

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
        await Shell.Current.GoToAsync($"{nameof(EditPickupLocationPage)}?EventId={mobEvent.Id}&PickupLocationId={PickupLocation.Id}");
    }
}
