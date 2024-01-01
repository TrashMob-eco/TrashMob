namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewEventSummaryViewModel : BaseViewModel
{
    public ViewEventSummaryViewModel(IMobEventManager mobEventManager)
    {
        EditEventSummaryCommand = new Command(async () => await EditEventSummary());
        AddPickupLocationCommand = new Command(async () => await AddPickupLocation());
        this.mobEventManager = mobEventManager;
    }

    [ObservableProperty]
    EventSummaryViewModel eventSummaryViewModel;
    private readonly IMobEventManager mobEventManager;

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new ObservableCollection<PickupLocationViewModel>();

    [ObservableProperty]
    bool enableEditEventSummary;

    [ObservableProperty]
    bool enableAddPickupLocation;

    public ICommand EditEventSummaryCommand { get; set; }
    public ICommand AddPickupLocationCommand { get; set; }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        var eventSummary = await mobEventManager.GetEventSummaryAsync(eventId);

        if (eventSummary != null)
        {

            EventSummaryViewModel = new EventSummaryViewModel
            {
                ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees,
                DurationInMinutes = eventSummary.DurationInMinutes,
                EventId = eventId,
                Notes = eventSummary.Notes,
                NumberOfBags = eventSummary.NumberOfBags,
            };
        }

        EnableEditEventSummary = mobEvent.IsEventLead();
        EnableAddPickupLocation = mobEvent.IsEventLead();

        IsBusy = false;
    }

    private async Task AddPickupLocation()
    {
        await Shell.Current.GoToAsync($"{nameof(AddPickupLocationPage)}?EventId={EventSummaryViewModel.EventId}");
    }

    private async Task EditEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventSummaryPage)}?EventId={EventSummaryViewModel.EventId}");
    }
}
