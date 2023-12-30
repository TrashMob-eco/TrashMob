namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMobMobile.Data;

public partial class ViewEventSummaryViewModel : BaseViewModel
{
    public ViewEventSummaryViewModel(IMobEventManager mobEventManager)
    {
        EditEventSummaryCommand = new Command(async () => await EditEventSummary());
        this.mobEventManager = mobEventManager;
    }

    [ObservableProperty]
    EventSummaryViewModel eventSummaryViewModel;
    private readonly IMobEventManager mobEventManager;

    public ICommand EditEventSummaryCommand { get; set; }

    public async Task Init(string eventId)
    {
        IsBusy = true;

        var eventSummary = await mobEventManager.GetEventSummaryAsync(new Guid(eventId));

        if (eventSummary != null)
        {
            EventSummaryViewModel = new EventSummaryViewModel
            {
                ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees,
                DurationInMinutes = eventSummary.DurationInMinutes,
                EventId = eventSummary.EventId,
                Notes = eventSummary.Notes,
                NumberOfBags = eventSummary.NumberOfBags,
            };
        }

        IsBusy = false;
    }

    private async Task EditEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventSummaryPage)}?EventId={eventSummaryViewModel.EventId}");
    }
}
