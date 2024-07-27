namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class EditPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;

    private readonly IPickupLocationManager pickupLocationManager = pickupLocationManager;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    private PickupLocation pickupLocation;

    [ObservableProperty]
    private PickupLocationViewModel pickupLocationViewModel;

    public async Task Init(Guid eventId, Guid pickupLocationId)
    {
        IsBusy = true;

        try
        {
            var mobEvent = await mobEventManager.GetEventAsync(eventId);

            EventViewModel = mobEvent.ToEventViewModel();

            pickupLocation = await pickupLocationManager.GetPickupLocationAsync(pickupLocationId);

            PickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager, NotificationService)
            {
                Name = pickupLocation.Name,
                Notes = pickupLocation.Notes,
            };

            await PickupLocationViewModel.Init(eventId);
            
            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError($"An error has occurred while loading the event. Please wait and try again in a moment.");
        }
    }

    [RelayCommand]
    private async Task SavePickupLocation()
    {
        IsBusy = true;

        try
        {
            pickupLocation.Notes = PickupLocationViewModel.Notes;
            pickupLocation.Name = PickupLocationViewModel.Name;

            var updatedPickupLocation = await pickupLocationManager.UpdatePickupLocationAsync(pickupLocation);

            IsBusy = false;

            await NotificationService.Notify("Pickup Location has been saved.");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while saving the Pickup Location. Please wait and try again in a moment.");
        }
    }
}