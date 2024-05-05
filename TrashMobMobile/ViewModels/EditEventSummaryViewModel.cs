namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;

public partial class EditEventSummaryViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    public EditEventSummaryViewModel(IMobEventManager mobEventManager)
    {
        SaveEventSummaryCommand = new Command(async () => await SaveEventSummary());
        this.mobEventManager = mobEventManager;
    }

    [ObservableProperty]
    EventSummaryViewModel eventSummaryViewModel;
    public ICommand SaveEventSummaryCommand { get; set; }

    private EventSummary EventSummary { get; set; }

    [ObservableProperty]
    private bool enableSaveEventSummary;

    public async Task Init(string eventId)
    {
        IsBusy = true;

        EventSummary = await mobEventManager.GetEventSummaryAsync(new Guid(eventId));

        if (EventSummary != null)
        {
            EventSummaryViewModel = new EventSummaryViewModel
            {
                ActualNumberOfAttendees = EventSummary.ActualNumberOfAttendees,
                DurationInMinutes = EventSummary.DurationInMinutes,
                EventId = EventSummary.EventId,
                Notes = EventSummary.Notes,
                NumberOfBags = EventSummary.NumberOfBags,
            };
        }

        EnableSaveEventSummary = true;

        IsBusy = false;
    }

    private async Task SaveEventSummary()
    {
        IsBusy = true;
        EventSummary.ActualNumberOfAttendees = eventSummaryViewModel.ActualNumberOfAttendees;
        EventSummary.NumberOfBags = eventSummaryViewModel.NumberOfBags;
        EventSummary.DurationInMinutes = eventSummaryViewModel.DurationInMinutes;
        EventSummary.Notes = eventSummaryViewModel.Notes;

        await mobEventManager.UpdateEventSummaryAsync(EventSummary);

        IsBusy = false;

        await Notify("Event Summary has been updated.");
    }
}
