namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class EditEventSummaryViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;

    [ObservableProperty]
    private bool enableSaveEventSummary;

    [ObservableProperty]
    private EventSummaryViewModel eventSummaryViewModel;

    public EditEventSummaryViewModel(IMobEventManager mobEventManager)
    {
        this.mobEventManager = mobEventManager;
    }

    private EventSummary EventSummary { get; set; }

    public async Task Init(string eventId)
    {
        IsBusy = true;

        try
        {
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
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotifyError($"An error has occured while loading the event summary. Please wait and try again in a moment.");
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task SaveEventSummary()
    {
        IsBusy = true;

        try
        {
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

            await Notify("Event Summary has been updated.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotifyError($"An error has occured while saving the event summary. Please wait and try again in a moment.");
        }

        IsBusy = false;
    }
}