namespace TrashMobMobileApp.Platforms.Android;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.Identity.Client;
using Plugin.CurrentActivity;
using TrashMobMobileApp.Authentication;

[Activity(Label = "TrashMobMobile", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    const int RequestLocationId = 0;

    readonly string[] LocationPermissions =
    {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

    static void AddServices(IServiceCollection services)
    {
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        //Window.SetStatusBarColor(Android.Graphics.Color.Argb(255, 150, 186, 0));
        CrossCurrentActivity.Current.Init(this, savedInstanceState);
        DependencyService.Register<IParentWindowLocatorService, AndroidParentWindowLocatorService>();

        base.OnCreate(savedInstanceState);

        //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        //Forms.Init(this, savedInstanceState);
        //Xamarin.FormsMaps.Init(this, savedInstanceState);

        //LoadApplication(new App(AddServices));
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        if (requestCode == RequestLocationId)
        {
            if ((grantResults.Length == 1) && (grantResults[0] == (int)Permission.Granted))
            {
                // Permissions granted - display a message
            }
            else
            {
                // Permissions denied - display a message
            }
        }
        else
        {
            //Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
