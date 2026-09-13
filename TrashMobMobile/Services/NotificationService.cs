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
        try
        {
            var toast = Toast.Make(message, toastDuration, fontSize);
            await toast.Show();
        }
        catch (Exception ex)
        {
            // Rendering a toast must never crash the app. CommunityToolkit.Maui's
            // Toast/Snackbar wrap platform-native widgets (Android Material
            // Components) that can fail to inflate on certain OEM/Android version
            // combinations (e.g. a theme attribute resolution failure) -- see the
            // same catch in NotifyError below for a concrete case that reached
            // production as a fatal crash.
            SentrySdk.CaptureException(ex);
        }
    }

    public async Task NotifyError(string message)
    {
        try
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
        catch (Exception ex)
        {
            // This is called from every catch block in BaseViewModel.ExecuteAsync to
            // show the user a friendly error message. If showing THAT message also
            // throws, it must not escape and crash the app -- that would turn any
            // handled error (a network blip, an HTTP error) into a fatal crash instead.
            // Confirmed in production: CommunityToolkit.Maui's Snackbar failed to
            // inflate Android's design_layout_snackbar_include layout with an
            // UnsupportedOperationException on a OnePlus 8 Pro (Android 11), and
            // because it was unguarded here, it crashed the whole app.
            SentrySdk.CaptureException(ex);
        }
    }
}