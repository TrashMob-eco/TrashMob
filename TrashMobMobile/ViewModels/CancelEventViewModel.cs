namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;

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
            EventId = EventViewModel.Id
        };

        await mobEventManager.DeleteEventAsync(cancellationRequest);

        IsBusy = false;

        await Notify("The event has been cancelled.");

        await Navigation.PopAsync();
    }
}