namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class PickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;

    private readonly IPickupLocationManager pickupLocationManager = pickupLocationManager;

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
            await NotificationService.NotifyError("An error has occurred while loading the event. Please wait and try again in a moment.");
        }
    }

    [RelayCommand]
    private async Task DeletePickupLocation()
    {
        try
        {
            await pickupLocationManager.DeletePickupLocationAsync(PickupLocation);

            await NotificationService.Notify("Pickup location has been removed.");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while deleting the pickup location. Please wait and try again in a moment.");
        }
    }

    [RelayCommand]
    private async Task EditPickupLocation()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(EditPickupLocationPage)}?EventId={mobEvent.Id}&PickupLocationId={PickupLocation.Id}");
    }
}