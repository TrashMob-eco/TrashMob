namespace TrashMobMobileApp.Platforms.MacCatalyst;

using TrashMobMobileApp.Authentication;
using UIKit;

public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        _ = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelperInstance.InitializePublicClientAppAsync()).Result;

        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}