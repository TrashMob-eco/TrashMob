namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class EditPickupLocationViewModel : BaseViewModel
{
    [ObservableProperty]
    PickupLocationViewModel pickupLocationViewModel;

    [ObservableProperty]
    EventViewModel eventViewModel;

    private PickupLocation pickupLocation;

    public EditPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
        SavePickupLocationCommand = new Command(async () => await SavePickupLocation());
        this.pickupLocationManager = pickupLocationManager;
        this.mobEventManager = mobEventManager;
    }

    public async Task Init(Guid eventId, Guid pickupLocationId)
    {
        IsBusy = true;

        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();

        pickupLocation = await pickupLocationManager.GetPickupLocationAsync(pickupLocationId);

        PickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager)
        {
            Name = pickupLocation.Name,
            Notes = pickupLocation.Notes
        };

        await PickupLocationViewModel.Init(eventId);

        IsBusy = false;
    }

    public ICommand SavePickupLocationCommand { get; set; }

    private readonly IPickupLocationManager pickupLocationManager;
    private readonly IMobEventManager mobEventManager;

    private async Task SavePickupLocation()
    {
        IsBusy = true;

        pickupLocation.Notes = PickupLocationViewModel.Notes;
        pickupLocation.Name = PickupLocationViewModel.Name;

        var updatedPickupLocation = await pickupLocationManager.UpdatePickupLocationAsync(pickupLocation);

        IsBusy = false;

        await Notify("Pickup Location has been saved.");
        await Navigation.PopAsync();
    }
}
