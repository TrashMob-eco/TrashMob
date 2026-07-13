namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Sentry;
using TrashMob.Models;
using TrashMobMobile.Pages;
using TrashMobMobile.Services;

/// <summary>
/// In-progress view for a running Instant Event. Init decides between two paths based on
/// persisted Preferences:
///  - Resume path: a prior Init already created an event and saved its id/start time.
///    Validate with the server, then rebuild the timer from the persisted start.
///  - Fresh path: capture GPS, create the event on the server, persist state for
///    later resume, start the timer.
/// The persistence handles the "user closed the app mid-pick" case — the running event
/// still exists on the server as Active, and this VM is where we teach the app to pick
/// it back up. See Planning/Projects/Project_65_Instant_Events.md Phase 1.
/// </summary>
public partial class InstantEventViewModel(
    IMobEventManager mobEventManager,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    // Preferences keys for a running Instant Event that survives app-close/reopen.
    // Cleared on Stop and on server-side confirmation that the event is no longer Active.
    public const string PrefKeyEventId = "instant_event_id";
    public const string PrefKeyStartedAt = "instant_event_started_at";

    private readonly IMobEventManager mobEventManager = mobEventManager;
    private DateTimeOffset startedAt;
    private IDispatcherTimer? timer;

    [ObservableProperty]
    private Guid eventId;

    [ObservableProperty]
    private string elapsedTime = "00:00:00";

    [ObservableProperty]
    private bool isRunning;

    [ObservableProperty]
    private bool isFailed;

    [ObservableProperty]
    private string statusMessage = "Getting location…";

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            // Resume path — a previously-started Instant Event may still be running
            // on the server if the user closed the app mid-pick.
            if (await TryResumeAsync())
            {
                return;
            }

            // Fresh path — no pending event; start a brand new one.
            await StartFreshAsync();
        }, "Failed to start Instant Event. Please try again.");
    }

    private async Task<bool> TryResumeAsync()
    {
        var persistedIdString = Preferences.Default.Get(PrefKeyEventId, string.Empty);
        if (string.IsNullOrEmpty(persistedIdString) || !Guid.TryParse(persistedIdString, out var persistedId))
        {
            return false;
        }

        var persistedStartedAtString = Preferences.Default.Get(PrefKeyStartedAt, string.Empty);
        if (string.IsNullOrEmpty(persistedStartedAtString)
            || !DateTimeOffset.TryParse(persistedStartedAtString, out var persistedStartedAt))
        {
            // Half-persisted state — clear and fall through to a fresh start.
            ClearPersistedState();
            return false;
        }

        StatusMessage = "Resuming your pick…";

        Event? serverEvent;
        try
        {
            serverEvent = await mobEventManager.GetEventAsync(persistedId);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            // Server unreachable or 404 — treat as no resume target and let user start fresh.
            ClearPersistedState();
            return false;
        }

        // If someone completed the event out-of-band (another device, admin action, or the
        // Phase 4 background-abandonment guard once we ship it), the server status will be
        // Complete or Canceled. Don't resume — clear state and let the user start fresh.
        if (serverEvent == null
            || serverEvent.EventStatusId == (int)EventStatusEnum.Complete
            || serverEvent.EventStatusId == (int)EventStatusEnum.Canceled)
        {
            ClearPersistedState();
            return false;
        }

        EventId = serverEvent.Id;
        startedAt = persistedStartedAt;
        IsRunning = true;
        StatusMessage = "Recording your pick";
        StartTimer();
        return true;
    }

    private async Task StartFreshAsync()
    {
        StatusMessage = "Getting location…";
        var location = await GetCurrentLocationAsync();

        if (location == null)
        {
            IsFailed = true;
            StatusMessage = "Location required. Check your device settings and try again.";
            return;
        }

        StatusMessage = "Starting your pick…";
        var mobEvent = await mobEventManager
            .AddInstantEventAsync(location.Latitude, location.Longitude);

        EventId = mobEvent.Id;
        startedAt = mobEvent.EventDate;
        PersistState(mobEvent.Id, mobEvent.EventDate);

        IsRunning = true;
        StatusMessage = "Recording your pick";
        StartTimer();
    }

    [RelayCommand]
    private async Task Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        StopTimer();
        IsRunning = false;

        // Capture EventId before ExecuteAsync so the navigation URL is stable if the
        // ViewModel state gets tossed while the API call is in flight.
        var completedEventId = EventId;

        await ExecuteAsync(async () =>
        {
            await mobEventManager.CompleteEventAsync(completedEventId);

            // Clear persisted state only after the server confirms Complete — if the
            // Complete call fails, we still want the user to be able to resume and try
            // again from the Dashboard.
            ClearPersistedState();

            await Shell.Current
                .GoToAsync($"//{nameof(EditEventSummaryPage)}?EventId={completedEventId}");
        }, "Failed to complete the Instant Event. You can complete it manually from your event history.");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        StopTimer();
        await Shell.Current.GoToAsync("..");
    }

    private void StartTimer()
    {
        timer = Application.Current!.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (_, _) =>
        {
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            if (elapsed < TimeSpan.Zero)
            {
                elapsed = TimeSpan.Zero;
            }

            ElapsedTime = elapsed.ToString(@"hh\:mm\:ss");
        };
        timer.Start();
    }

    private void StopTimer()
    {
        timer?.Stop();
        timer = null;
    }

    private static void PersistState(Guid eventId, DateTimeOffset startedAt)
    {
        Preferences.Default.Set(PrefKeyEventId, eventId.ToString());
        Preferences.Default.Set(PrefKeyStartedAt, startedAt.ToString("o"));
    }

    private static void ClearPersistedState()
    {
        Preferences.Default.Remove(PrefKeyEventId);
        Preferences.Default.Remove(PrefKeyStartedAt);
    }

    private static async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            return await Geolocation.Default.GetLocationAsync(request);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return null;
        }
    }
}
