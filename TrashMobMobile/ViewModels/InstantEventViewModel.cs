namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Sentry;
using TrashMob.Models;
using TrashMobMobile.Pages;
using TrashMobMobile.Services;
using TrashMobMobile.Services.Offline;

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
    IRouteRecordingCoordinator routeCoordinator,
    IUserManager userManager,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    // Preferences keys for a running Instant Event that survives app-close/reopen.
    // Cleared on Stop and on server-side confirmation that the event is no longer Active.
    public const string PrefKeyEventId = "instant_event_id";
    public const string PrefKeyStartedAt = "instant_event_started_at";

    // Persisted per-event opt-in for route tracking. Set from the query parameter on
    // fresh Start. On resume (persisted event id) we re-read this to know whether the
    // in-flight event had tracking enabled — the coordinator state already lives in
    // RouteTrackingSessionManager, but we still need the flag for UI (e.g. showing
    // the map view).
    public const string PrefKeyTrackRoute = "instant_event_track_route";

    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IRouteRecordingCoordinator routeCoordinator = routeCoordinator;
    private readonly IUserManager userManager = userManager;
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
    private bool isTrackingRoute;

    [ObservableProperty]
    private string statusMessage = "Getting location…";

    /// <summary>
    /// Populated with GPS points as they arrive when route tracking is enabled. Bound to
    /// the map view on <see cref="Pages.InstantEventPage"/> for live route rendering.
    /// </summary>
    public ObservableCollection<Location> Locations { get; } = [];

    /// <summary>
    /// Set by <see cref="Pages.InstantEventPage"/> from the TrackRoute query parameter
    /// before Init runs. Determines whether a fresh Start also opens a route session.
    /// Resume path reads Preferences instead (the query param isn't passed on resume).
    /// </summary>
    public bool RequestedTrackRoute { get; set; }

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

        // Route-tracking resume is best-effort. The coordinator's in-memory state
        // (listener CTS, active session id, in-progress point collection) doesn't
        // survive an app-close, so on a fresh boot IsRecording will be false and we
        // can't rejoin the GPS listener seamlessly. Any GPS points captured before
        // the close are safe in SQLite (RoutePointWriter flushes on process shutdown
        // best-effort, and SyncQueue.GetInterruptedSessionsAsync surfaces stragglers
        // on next app boot). Full route-recording resume would need broader work in
        // RouteTrackingSessionManager.TryRestoreSession + coordinator boot hydration;
        // out of scope for this Phase 2 slice.
        IsTrackingRoute = routeCoordinator.IsRecording && routeCoordinator.ActiveEventId == serverEvent.Id;
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
        PersistState(mobEvent.Id, mobEvent.EventDate, RequestedTrackRoute);

        IsRunning = true;
        StatusMessage = "Recording your pick";
        StartTimer();

        if (RequestedTrackRoute)
        {
            await StartRouteTrackingAsync(mobEvent);
        }
    }

    private async Task StartRouteTrackingAsync(Event mobEvent)
    {
        var result = await routeCoordinator.StartAsync(
            mobEvent.Id,
            mobEvent.Name,
            userManager.CurrentUser.Id,
            startedAt,
            skipDefaultTrim: false,
            Locations);

        if (result.Outcome == RouteRecordingStartOutcome.Success)
        {
            IsTrackingRoute = true;
        }
        else if (result.Outcome == RouteRecordingStartOutcome.ConflictOtherEvent)
        {
            // Another event is currently being tracked — surface a soft warning but
            // let the Instant Event continue running without a route. Turning off the
            // route toggle for this session is preferable to failing the whole flow.
            await NotificationService.Notify(
                $"Route tracking is already active for '{result.ConflictingEventName}'. Your Instant Event is running without a route.");
        }
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
            // Stop the route recording first so its cleanup (flush writer, mark session
            // pending upload, POST route) runs even if the CompleteEvent call fails.
            // The route is separately valuable from the completed-event state.
            if (routeCoordinator.IsRecording && routeCoordinator.ActiveEventId == completedEventId)
            {
                await routeCoordinator.StopAsync(DateTimeOffset.UtcNow);
            }

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

    private static void PersistState(Guid eventId, DateTimeOffset startedAt, bool trackRoute)
    {
        Preferences.Default.Set(PrefKeyEventId, eventId.ToString());
        Preferences.Default.Set(PrefKeyStartedAt, startedAt.ToString("o"));
        Preferences.Default.Set(PrefKeyTrackRoute, trackRoute);
    }

    private static void ClearPersistedState()
    {
        Preferences.Default.Remove(PrefKeyEventId);
        Preferences.Default.Remove(PrefKeyStartedAt);
        Preferences.Default.Remove(PrefKeyTrackRoute);
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
