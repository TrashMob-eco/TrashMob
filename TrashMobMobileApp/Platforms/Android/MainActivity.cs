namespace TrashMobMobileApp.Platforms.Android;

using global::Android;
using global::Android.App;
using global::Android.Content;
using global::Android.Content.PM;
using global::Android.OS;
using global::TrashMobMobileApp.Authentication;
using Microsoft.Identity.Client;

[Activity(Theme = "@style/TMTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    const int RequestLocationId = 0;

    readonly string[] LocationPermissions =
    {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

    protected override void OnCreate(Bundle savedInstanceState)
    {
        //Window.SetStatusBarColor(Android.Graphics.Color.Argb(255, 150, 186, 0));
        base.OnCreate(savedInstanceState);
        //CrossCurrentActivity.Current.Init(this, savedInstanceState);
        //DependencyService.Register<IParentWindowLocatorService, AndroidParentWindowLocatorService>();
        PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelperInstance.AzureADB2CConfig.ClientId}://auth";
        PlatformConfig.Instance.ParentWindow = this;

        // Initialize MSAL and platformConfig is set
        _ = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelperInstance.InitializePublicClientAppAsync()).Result;
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }

    protected override void OnStart()
    {
        base.OnStart();

        if ((int)Build.VERSION.SdkInt >= 23)
        {
            if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                RequestPermissions(LocationPermissions, RequestLocationId);
            }
            else
            {
                // Permissions already granted - display a message
            }
        }
    }
}
