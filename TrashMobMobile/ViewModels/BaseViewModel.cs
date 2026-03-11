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

    protected async Task ExecuteAsync(Func<Task> operation, string errorMessage)
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
            await NotificationService.NotifyError(string.IsNullOrEmpty(ex.Message) ? errorMessage : ex.Message);
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
            await NotificationService.NotifyError($"{errorMessage} ({ex.GetType().Name}: {ex.Message})");
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string errorMessage)
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
            await NotificationService.NotifyError(string.IsNullOrEmpty(ex.Message) ? errorMessage : ex.Message);
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
            await NotificationService.NotifyError($"{errorMessage} ({ex.GetType().Name}: {ex.Message})");
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}