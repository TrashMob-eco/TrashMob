namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;

public partial class EditEventSummaryViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    public EditEventSummaryViewModel(IMobEventManager mobEventManager)
    {
        this.mobEventManager = mobEventManager;
    }

    [ObservableProperty]
    EventSummaryViewModel eventSummaryViewModel;

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

    [RelayCommand]
    private async Task SaveEventSummary()
    {
        IsBusy = true;
        EventSummary.ActualNumberOfAttendees = EventSummaryViewModel.ActualNumberOfAttendees;
        EventSummary.NumberOfBags = EventSummaryViewModel.NumberOfBags;
        EventSummary.DurationInMinutes = EventSummaryViewModel.DurationInMinutes;
        EventSummary.Notes = EventSummaryViewModel.Notes;

        if (EventSummary.CreatedByUserId == Guid.Empty)
        {
            EventSummary.CreatedByUserId = App.CurrentUser.Id;
            await mobEventManager.AddEventSummaryAsync(EventSummary);
        }
        else
        {
            await mobEventManager.UpdateEventSummaryAsync(EventSummary);
        }

        IsBusy = false;

        await Notify("Event Summary has been updated.");
    }
}
