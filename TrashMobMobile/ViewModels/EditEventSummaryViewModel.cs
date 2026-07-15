namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sentry;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class EditEventSummaryViewModel(
    IMobEventManager mobEventManager,
    INotificationService notificationService,
    IUserManager userManager,
    IEventAttendeeRouteRestService eventAttendeeRouteRestService,
    ICommunityRestService communityRestService) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IUserManager userManager = userManager;
    private readonly IEventAttendeeRouteRestService eventAttendeeRouteRestService = eventAttendeeRouteRestService;
    private readonly ICommunityRestService communityRestService = communityRestService;

    [ObservableProperty]
    private bool enableSaveEventSummary;

    [ObservableProperty]
    private EventSummaryViewModel eventSummaryViewModel = new();

    [ObservableProperty]
    private WeightUnit selectedWeightUnit = null!;

    [ObservableProperty]
    private bool isFromRouteData;

    // Community-contribution banner (Project 65 Phase 3). Populated after Init if the
    // event's GPS falls inside an enabled community's bounds. Purely informational —
    // the community's stats aggregate this event automatically via the same bounding
    // box, so there's no explicit opt-in step needed. See
    // Planning/Projects/Project_65_Instant_Events.md Phase 3.
    [ObservableProperty]
    private string matchedCommunityName = string.Empty;

    [ObservableProperty]
    private string matchedCommunitySlug = string.Empty;

    [ObservableProperty]
    private bool hasMatchedCommunity;

    public ObservableCollection<WeightUnit> WeightUnits { get; } =
    [
        new WeightUnit { Id = (int)WeightUnitEnum.Pound, Name = "lbs", Description = "Pounds" },
        new WeightUnit { Id = (int)WeightUnitEnum.Kilogram, Name = "kg", Description = "Kilograms" },
    ];

    private EventSummary EventSummary { get; set; } = new EventSummary();

    public async Task Init(string eventId)
    {
        await ExecuteAsync(async () =>
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

                IsFromRouteData = EventSummary.IsFromRouteData;

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

                // Pre-fill from route data if this is a new summary
                if (EventSummary.CreatedByUserId == Guid.Empty)
                {
                    await TryPrefillFromRouteData(new Guid(eventId));
                }
            }

            EnableSaveEventSummary = true;

            await TryLoadMatchedCommunityAsync(new Guid(eventId));
        }, "An error has occurred while loading the event summary. Please wait and try again in a moment.");
    }

    private async Task TryLoadMatchedCommunityAsync(Guid eventId)
    {
        try
        {
            var mobEvent = await mobEventManager.GetEventAsync(eventId);
            if (mobEvent?.Latitude is not { } lat || mobEvent.Longitude is not { } lng)
            {
                return;
            }

            var match = await communityRestService.GetCommunityAtLocationAsync(lat, lng);
            if (match != null)
            {
                MatchedCommunityName = match.Name;
                MatchedCommunitySlug = match.Slug;
                HasMatchedCommunity = true;
            }
        }
        catch (Exception ex)
        {
            // Non-critical — the community banner is purely informational, don't break
            // the summary screen if the lookup fails.
            SentrySdk.CaptureException(ex);
        }
    }

    private async Task TryPrefillFromRouteData(Guid eventId)
    {
        try
        {
            var targetUnitId = SelectedWeightUnit?.Id ?? (int)WeightUnitEnum.Pound;
            var prefill = await eventAttendeeRouteRestService.GetEventSummaryPrefillAsync(eventId, targetUnitId);

            if (prefill is { HasRouteData: true })
            {
                EventSummaryViewModel.NumberOfBags = prefill.NumberOfBags;
                EventSummaryViewModel.PickedWeight = prefill.PickedWeight;
                EventSummaryViewModel.DurationInMinutes = prefill.DurationInMinutes;
                EventSummaryViewModel.ActualNumberOfAttendees = prefill.ActualNumberOfAttendees;
                IsFromRouteData = true;
            }
        }
        catch
        {
            // Pre-fill is best effort — don't fail the summary load
        }
    }

    [RelayCommand]
    private async Task SaveEventSummary()
    {
        await ExecuteAsync(async () =>
        {
            EventSummary.ActualNumberOfAttendees = EventSummaryViewModel.ActualNumberOfAttendees;
            EventSummary.NumberOfBags = EventSummaryViewModel.NumberOfBags;
            EventSummary.DurationInMinutes = EventSummaryViewModel.DurationInMinutes;
            EventSummary.Notes = EventSummaryViewModel.Notes;
            EventSummary.PickedWeight = EventSummaryViewModel.PickedWeight;
            EventSummary.PickedWeightUnitId = SelectedWeightUnit?.Id ?? (int)WeightUnitEnum.Pound;
            EventSummary.IsFromRouteData = IsFromRouteData;

            if (EventSummary.CreatedByUserId == Guid.Empty)
            {
                EventSummary.CreatedByUserId = userManager.CurrentUser.Id;
                await mobEventManager.AddEventSummaryAsync(EventSummary);
            }
            else
            {
                await mobEventManager.UpdateEventSummaryAsync(EventSummary);
            }

            await NotificationService.Notify("Event Summary has been updated.");
            await Shell.Current.GoToAsync("..");
        }, "An error has occurred while saving the event summary. Please wait and try again in a moment.");
    }
}