namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

public partial class CancelEventViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    public CancelEventViewModel(IMobEventManager mobEventManager)
    {
        this.mobEventManager = mobEventManager;
    }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            var mobEvent = await mobEventManager.GetEventAsync(eventId);

            EventViewModel = mobEvent.ToEventViewModel();
        }
        catch
        {
            await NotifyError($"An error has occured while loading the event. Please wait and try again in a moment.");
        }

        IsBusy = false;
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

            await Notify("The event has been cancelled.");

            await Navigation.PopAsync();
        }
        catch
        {
            await NotifyError($"An error has occured while cancelling the event. Please wait and try again in a moment.");
        }
    }
}