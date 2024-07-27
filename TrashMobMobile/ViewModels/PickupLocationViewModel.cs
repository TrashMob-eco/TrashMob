namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

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

        try
        {
            mobEvent = await mobEventManager.GetEventAsync(eventId);

            CanDeletePickupLocation = mobEvent.IsEventLead();
            CanEditPickupLocation = mobEvent.IsEventLead();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error has occured while loading the event. Please wait and try again in a moment.");
        }
    }

    [RelayCommand]
    private async Task DeletePickupLocation()
    {
        try
        {
            await pickupLocationManager.DeletePickupLocationAsync(PickupLocation);

            await Notify("Pickup location has been removed.");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error has occured while deletign the pickup location. Please wait and try again in a moment.");
        }
    }

    [RelayCommand]
    private async Task EditPickupLocation()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(EditPickupLocationPage)}?EventId={mobEvent.Id}&PickupLocationId={PickupLocation.Id}");
    }
}