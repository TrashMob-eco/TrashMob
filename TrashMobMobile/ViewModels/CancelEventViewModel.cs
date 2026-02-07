namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

public partial class CancelEventViewModel(IMobEventManager mobEventManager,
                                          INotificationService notificationService,
                                          IUserManager userManager) 
    : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly INotificationService notificationService = notificationService;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private EventViewModel eventViewModel = new();

    public async Task Init(Guid eventId)
    {
        await ExecuteAsync(async () =>
        {
            var mobEvent = await mobEventManager.GetEventAsync(eventId);
            EventViewModel = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);
        }, "An error has occurred while loading the event. Please wait and try again in a moment.");
    }

    [RelayCommand]
    private async Task CancelEvent()
    {
        await ExecuteAsync(async () =>
        {
            var cancellationRequest = new EventCancellationRequest
            {
                CancellationReason = EventViewModel.CancellationReason,
                EventId = EventViewModel.Id,
            };

            await mobEventManager.DeleteEventAsync(cancellationRequest);
            await notificationService.Notify("The event has been cancelled.");
            await Navigation.PopAsync();
        }, "An error has occurred while cancelling the event. Please wait and try again in a moment.");
    }
}