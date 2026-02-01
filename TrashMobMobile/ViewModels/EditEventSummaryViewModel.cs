namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class EditEventSummaryViewModel(IMobEventManager mobEventManager, INotificationService notificationService, IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private bool enableSaveEventSummary;

    [ObservableProperty]
    private EventSummaryViewModel eventSummaryViewModel = new();

    [ObservableProperty]
    private WeightUnit selectedWeightUnit;

    public ObservableCollection<WeightUnit> WeightUnits { get; } =
    [
        new WeightUnit { Id = (int)WeightUnitEnum.Pound, Name = "lbs", Description = "Pounds" },
        new WeightUnit { Id = (int)WeightUnitEnum.Kilogram, Name = "kg", Description = "Kilograms" },
    ];

    private EventSummary EventSummary { get; set; } = new EventSummary();

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
                    PickedWeight = EventSummary.PickedWeight,
                    PickedWeightUnitId = EventSummary.PickedWeightUnitId,
                };

                // Set selected weight unit based on saved value or user preference
                var savedUnitId = EventSummary.PickedWeightUnitId;
                if (savedUnitId > 0)
                {
                    SelectedWeightUnit = WeightUnits.FirstOrDefault(u => u.Id == savedUnitId) ?? WeightUnits[0];
                }
                else
                {
                    // Default based on user preference
                    var defaultUnitId = userManager.CurrentUser?.PrefersMetric == true
                        ? (int)WeightUnitEnum.Kilogram
                        : (int)WeightUnitEnum.Pound;
                    SelectedWeightUnit = WeightUnits.FirstOrDefault(u => u.Id == defaultUnitId) ?? WeightUnits[0];
                }
            }

            EnableSaveEventSummary = true;

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError($"An error has occurred while loading the event summary. Please wait and try again in a moment.");
        }
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
            EventSummary.PickedWeight = EventSummaryViewModel.PickedWeight;
            EventSummary.PickedWeightUnitId = SelectedWeightUnit?.Id ?? (int)WeightUnitEnum.Pound;

            if (EventSummary.CreatedByUserId == Guid.Empty)
            {
                EventSummary.CreatedByUserId = userManager.CurrentUser.Id;
                await mobEventManager.AddEventSummaryAsync(EventSummary);
            }
            else
            {
                await mobEventManager.UpdateEventSummaryAsync(EventSummary);
            }

            IsBusy = false;

            await NotificationService.Notify("Event Summary has been updated.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError($"An error has occurred while saving the event summary. Please wait and try again in a moment.");
        }
    }
}