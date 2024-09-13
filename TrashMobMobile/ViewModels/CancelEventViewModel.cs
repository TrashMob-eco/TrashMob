namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

public partial class CancelEventViewModel(IMobEventManager mobEventManager,
    INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly INotificationService notificationService = notificationService;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            var mobEvent = await mobEventManager.GetEventAsync(eventId);

            EventViewModel = mobEvent.ToEventViewModel();

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
    private async Task CancelEvent()
    {
        IsBusy = true;

        var cancellationRequest = new EventCancellationRequest
        {
            CancellationReason = EventViewModel.CancellationReason,
            EventId = EventViewModel.Id,
        };

        try
        {
            await mobEventManager.DeleteEventAsync(cancellationRequest);

            IsBusy = false;

            await notificationService.Notify("The event has been cancelled.");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotificationService.NotifyError($"An error has occurred while cancelling the event. Please wait and try again in a moment.");
        }
    }
}