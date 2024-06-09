namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class EditPickupLocationViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    private readonly IPickupLocationManager pickupLocationManager;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    private PickupLocation pickupLocation;

    [ObservableProperty]
    private PickupLocationViewModel pickupLocationViewModel;

    public EditPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
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
            Notes = pickupLocation.Notes,
        };

        await PickupLocationViewModel.Init(eventId);

        IsBusy = false;
    }

    [RelayCommand]
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