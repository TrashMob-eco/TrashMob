namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;

public partial class CancelEventViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    public CancelEventViewModel(IMobEventManager mobEventManager)
    {
        CancelEventCommand = new Command(async () => await CancelEvent());
        this.mobEventManager = mobEventManager;
    }

    [ObservableProperty]
    EventViewModel eventViewModel;

    public ICommand CancelEventCommand { get; set; }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();

        IsBusy = false;
    }

    private async Task CancelEvent()
    {
        IsBusy = true;

        var cancellationRequest = new EventCancellationRequest()
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
