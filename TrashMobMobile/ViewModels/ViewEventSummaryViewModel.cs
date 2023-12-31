namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

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

    [ObservableProperty]
    bool enableEditEventSummary;

    public ICommand EditEventSummaryCommand { get; set; }

    public async Task Init(string eventId)
    {
        IsBusy = true;

        var mobEvent = await mobEventManager.GetEventAsync(new Guid(eventId));

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

        EnableEditEventSummary = mobEvent.IsEventLead();

        IsBusy = false;
    }

    private async Task EditEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventSummaryPage)}?EventId={eventSummaryViewModel.EventId}");
    }
}
