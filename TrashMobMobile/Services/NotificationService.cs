namespace TrashMobMobile.Services;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public interface INotificationService
{
    Task Notify(string message, double fontSize = 14, ToastDuration toastDuration = ToastDuration.Short);

    Task NotifyError(string message);
}

public class NotificationService : INotificationService
{
    public async Task Notify(string message, double fontSize = 14, ToastDuration toastDuration = ToastDuration.Short)
    {
        var toast = Toast.Make(message, toastDuration, fontSize);
        await toast.Show();
    }

    public async Task NotifyError(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.Red,
            TextColor = Colors.White,
            CornerRadius = new CornerRadius(10),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
        };

        var text = message;
        var duration = TimeSpan.FromSeconds(3);

        var snackbar = Snackbar.Make(text, duration: duration, visualOptions: snackbarOptions);
        await snackbar.Show(cancellationTokenSource.Token);
    }
}