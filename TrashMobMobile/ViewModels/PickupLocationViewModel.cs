namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class PickupLocationViewModel : BaseViewModel
{
    public PickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
        DeletePickupLocationCommand = new Command(async () => await DeletePickupLocation());
        EditPickupLocationCommand = new Command(async () => await EditPickupLocation());
        this.pickupLocationManager = pickupLocationManager;
        this.mobEventManager = mobEventManager;
    }

    public ICommand DeletePickupLocationCommand { get; set; }
    public ICommand EditPickupLocationCommand { get; set; }

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

    private readonly IPickupLocationManager pickupLocationManager;
    private readonly IMobEventManager mobEventManager;

    private async Task DeletePickupLocation()
    {        
        await pickupLocationManager.DeletePickupLocationAsync(PickupLocation);

        await Notify("Pickup location has been removed.");

        await Navigation.PopAsync();
    }

    private async Task EditPickupLocation()
    {
        await Shell.Current.GoToAsync($"{nameof(EditPickupLocationPage)}?EventId={mobEvent.Id}&PickupLocationId={PickupLocation.Id}");
    }
}
