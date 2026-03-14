namespace TrashMobMobile.ViewModels;

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Services;

public abstract partial class BaseViewModel(INotificationService notificationService) : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isError;

    public INavigation Navigation { get; set; } = null!;

    public INotificationService NotificationService { get; } = notificationService;

    protected async Task ExecuteAsync(Func<Task> operation, string errorMessage, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        IsError = false;

        try
        {
            await operation();
        }
        catch (HttpRequestException ex) when (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine($"[ExecuteAsync] Network error: {ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("No internet connection. Please check your network and try again.");
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[ExecuteAsync] HTTP error: {ex.StatusCode} - {ex.Message}\n{ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(errorMessage);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Debug.WriteLine("[ExecuteAsync] Operation cancelled by user.");
        }
        catch (TaskCanceledException ex)
        {
            Debug.WriteLine($"[ExecuteAsync] Timeout: {ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("The request timed out. Please try again.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ExecuteAsync] Unhandled {ex.GetType().Name}: {ex.Message}\n{ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(errorMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string errorMessage, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        IsError = false;

        try
        {
            return await operation();
        }
        catch (HttpRequestException ex) when (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Debug.WriteLine($"[ExecuteAsync<T>] Network error: {ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("No internet connection. Please check your network and try again.");
            return default;
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[ExecuteAsync<T>] HTTP error: {ex.StatusCode} - {ex.Message}\n{ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(errorMessage);
            return default;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Debug.WriteLine("[ExecuteAsync<T>] Operation cancelled by user.");
            return default;
        }
        catch (TaskCanceledException ex)
        {
            Debug.WriteLine($"[ExecuteAsync<T>] Timeout: {ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("The request timed out. Please try again.");
            return default;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ExecuteAsync<T>] Unhandled {ex.GetType().Name}: {ex.Message}\n{ex}");
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(errorMessage);
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
