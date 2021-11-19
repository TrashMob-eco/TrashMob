namespace TrashMobMobile.iOS
{
    using Foundation;
    using Microsoft.Extensions.DependencyInjection;
    using UIKit;
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Analytics;
    using Microsoft.AppCenter.Crashes;

    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        static void AddServices(IServiceCollection services)
        {
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();

            AppCenter.Start("9b6b51bd-d13e-47d7-b5f6-bf019340391a",
                   typeof(Analytics), typeof(Crashes));
            
            LoadApplication(new App(AddServices));

            return base.FinishedLaunching(app, options);
        }
    }
}
