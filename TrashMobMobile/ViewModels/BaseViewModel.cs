namespace TrashMobMobile.ViewModels;

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
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("No internet connection. Please check your network and try again.");
        }
        catch (HttpRequestException ex)
        {
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(string.IsNullOrEmpty(ex.Message) ? errorMessage : ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("The request timed out. Please try again.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(errorMessage);
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
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("No internet connection. Please check your network and try again.");
            return default;
        }
        catch (HttpRequestException ex)
        {
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError(string.IsNullOrEmpty(ex.Message) ? errorMessage : ex.Message);
            return default;
        }
        catch (TaskCanceledException ex)
        {
            SentrySdk.CaptureException(ex);
            IsError = true;
            await NotificationService.NotifyError("The request timed out. Please try again.");
            return default;
        }
        catch (Exception ex)
        {
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