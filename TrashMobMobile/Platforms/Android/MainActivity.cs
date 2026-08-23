namespace TrashMobMobile
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Microsoft.Identity.Client;

#pragma warning disable SA1118
    // LaunchMode must be SingleTask, not SingleTop. SingleTop only dedupes when the
    // existing instance is at the top of the *same* task; some OEM launchers (OnePlus/
    // OxygenOS observed in Sentry) start a new task when the app icon is tapped after
    // backgrounding, spinning up a second MainActivity while the first is still alive.
    // That second instance's Window collides with the MAUI Window already bound to the
    // first, throwing "This window is already associated with an active Activity."
    // SingleTask guarantees a single instance across all tasks/launch paths.
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
#pragma warning restore SA1118
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}