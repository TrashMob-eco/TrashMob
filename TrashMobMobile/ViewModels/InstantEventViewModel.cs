namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Sentry;
using TrashMobMobile.Pages;
using TrashMobMobile.Services;

/// <summary>
/// In-progress view for a running Instant Event. Init captures GPS + creates the event on the
/// server, then a 1-second dispatcher timer drives the elapsed-time display. Stop calls the
/// backend Complete endpoint and hands off to the existing EditEventSummaryPage for stats entry.
/// See Planning/Projects/Project_65_Instant_Events.md Phase 1.
/// </summary>
public partial class InstantEventViewModel(
    IMobEventManager mobEventManager,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
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
            IsRunning = true;
            StatusMessage = "Recording your pick";
            StartTimer();
        }, "Failed to start Instant Event. Please try again.");
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
