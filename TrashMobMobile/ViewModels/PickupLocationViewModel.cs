namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class PickupLocationViewModel(IPickupLocationManager pickupLocationManager,
                                             IMobEventManager mobEventManager, 
                                             INotificationService notificationService,
                                             IUserManager userManager)
    : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IUserManager userManager = userManager;
    private readonly IPickupLocationManager pickupLocationManager = pickupLocationManager;

    [ObservableProperty]
    private AddressViewModel address = new();

    [ObservableProperty]
    private bool canDeletePickupLocation;

    [ObservableProperty]
    private bool canEditPickupLocation;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string imageUrl = string.Empty;

    private Event mobEvent = new();

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string notes = string.Empty;

    public PickupLocation PickupLocation { get; set; } = new();

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            mobEvent = await mobEventManager.GetEventAsync(eventId);

            CanDeletePickupLocation = mobEvent.IsEventLead(userManager.CurrentUser.Id);
            CanEditPickupLocation = mobEvent.IsEventLead(userManager.CurrentUser.Id);

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