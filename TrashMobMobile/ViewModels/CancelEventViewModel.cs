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

        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();

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

        await mobEventManager.DeleteEventAsync(cancellationRequest);

        IsBusy = false;

        await toastService.Notify("The event has been cancelled.");

        await Navigation.PopAsync();
    }
}