namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

public partial class CancelEventViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private readonly IToastService toastService;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    public CancelEventViewModel(IMobEventManager mobEventManager,
        IToastService toastService)
    {
        this.mobEventManager = mobEventManager;
        this.toastService = toastService;
    }

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
            await NotifyError($"An error has occured while loading the event. Please wait and try again in a moment.");
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

            await toastService.Notify("The event has been cancelled.");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotifyError($"An error has occured while cancelling the event. Please wait and try again in a moment.");
        }
    }
}