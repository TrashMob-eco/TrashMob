namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;
using TrashMobMobile.Services.Offline;

public partial class SyncStatusViewModel(
    SyncQueue syncQueue,
    SyncService syncService,
    INotificationService notificationService) : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private bool hasPendingItems;

    [ObservableProperty]
    private bool hasNoPendingItems;

    [ObservableProperty]
    private string pendingSummary = string.Empty;

    [ObservableProperty]
    private bool isSyncing;

    public ObservableCollection<SyncItemViewModel> PendingItems { get; } = [];

    public async Task Init()
    {
        await LoadPendingItems();
    }

    [RelayCommand]
    private async Task RetryAll()
    {
        if (IsSyncing)
        {
            return;
        }

        IsSyncing = true;

        try
        {
            var synced = await syncService.SyncNowAsync();
            if (synced > 0)
            {
                await NotificationService.Notify($"{synced} item{(synced > 1 ? "s" : "")} synced successfully.");
            }
            else
            {
                await NotificationService.Notify("No items to sync or no connectivity.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SyncStatusViewModel: RetryAll failed: {ex.Message}");
            await NotificationService.NotifyError("Sync failed. Please try again.");
        }
        finally
        {
            IsSyncing = false;
            await LoadPendingItems();
        }
    }

    [RelayCommand]
    private async Task DiscardItem(SyncItemViewModel? item)
    {
        if (item == null)
        {
            return;
        }

        switch (item.ItemType)
        {
            case SyncItemType.Route:
                await syncQueue.DiscardSessionAsync(item.ItemId);
                break;
        }

        PendingItems.Remove(item);
        UpdateSummary();
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadPendingItems();
    }

    private async Task LoadPendingItems()
    {
        PendingItems.Clear();

        // Load route sessions
        var routeSessions = await syncQueue.GetSessionsReadyForUploadAsync();
        foreach (var session in routeSessions)
        {
            PendingItems.Add(new SyncItemViewModel
            {
                ItemId = session.SessionId,
                ItemType = SyncItemType.Route,
                Description = $"Route for event {session.EventId[..8]}...",
                Status = session.Status,
                LastError = session.LastError,
                RetryCount = session.RetryCount,
                CreatedAt = session.CreatedAt,
                CanDiscard = true,
            });
        }

        // Load interrupted (recording) sessions
        var interrupted = await syncQueue.GetInterruptedSessionsAsync();
        foreach (var session in interrupted)
        {
            PendingItems.Add(new SyncItemViewModel
            {
                ItemId = session.SessionId,
                ItemType = SyncItemType.Route,
                Description = $"Interrupted route for event {session.EventId[..8]}...",
                Status = session.Status,
                CreatedAt = session.CreatedAt,
                CanDiscard = true,
            });
        }

        // Load pending metrics
        var metrics = await syncQueue.GetMetricsReadyForUploadAsync();
        foreach (var item in metrics)
        {
            PendingItems.Add(new SyncItemViewModel
            {
                ItemId = item.Id.ToString(),
                ItemType = SyncItemType.Metrics,
                Description = $"Impact metrics for event {item.EventId[..8]}...",
                Status = item.Status,
                LastError = item.LastError,
                RetryCount = item.RetryCount,
                CreatedAt = item.CreatedAt,
            });
        }

        // Load pending photos
        var photos = await syncQueue.GetPhotosReadyForUploadAsync();
        foreach (var item in photos)
        {
            PendingItems.Add(new SyncItemViewModel
            {
                ItemId = item.Id.ToString(),
                ItemType = SyncItemType.Photo,
                Description = $"Photo for event {item.EventId[..8]}...",
                Status = item.Status,
                LastError = item.LastError,
                RetryCount = item.RetryCount,
                CreatedAt = item.CreatedAt,
            });
        }

        UpdateSummary();
    }

    private void UpdateSummary()
    {
        HasPendingItems = PendingItems.Count > 0;
        HasNoPendingItems = !HasPendingItems;

        if (HasPendingItems)
        {
            var routes = PendingItems.Count(i => i.ItemType == SyncItemType.Route);
            var metricsCount = PendingItems.Count(i => i.ItemType == SyncItemType.Metrics);
            var photosCount = PendingItems.Count(i => i.ItemType == SyncItemType.Photo);

            var parts = new List<string>();
            if (routes > 0)
            {
                parts.Add($"{routes} route{(routes > 1 ? "s" : "")}");
            }

            if (metricsCount > 0)
            {
                parts.Add($"{metricsCount} metric{(metricsCount > 1 ? "s" : "")}");
            }

            if (photosCount > 0)
            {
                parts.Add($"{photosCount} photo{(photosCount > 1 ? "s" : "")}");
            }

            PendingSummary = $"{PendingItems.Count} pending: {string.Join(", ", parts)}";
        }
        else
        {
            PendingSummary = "All data synced";
        }
    }
}

public enum SyncItemType
{
    Route,
    Metrics,
    Photo,
}

public class SyncItemViewModel
{
    public string ItemId { get; set; } = string.Empty;

    public SyncItemType ItemType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? LastError { get; set; }

    public int RetryCount { get; set; }

    public string CreatedAt { get; set; } = string.Empty;

    public bool CanDiscard { get; set; }

    public bool HasError => !string.IsNullOrEmpty(LastError);

    public string TypeLabel => ItemType switch
    {
        SyncItemType.Route => "Route",
        SyncItemType.Metrics => "Metrics",
        SyncItemType.Photo => "Photo",
        _ => "Unknown",
    };

    public string StatusDisplay => Status switch
    {
        "PendingUpload" => "Pending",
        "Failed" => $"Failed (retry {RetryCount})",
        "PermanentlyFailed" => "Permanently Failed",
        "Recording" => "Interrupted",
        _ => Status,
    };
}
