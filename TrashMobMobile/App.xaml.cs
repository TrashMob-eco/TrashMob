namespace TrashMobMobile;

using Syncfusion.Licensing;
using TrashMob.Models;
using TrashMobMobile.Config;

public partial class App : Application
{
    public static User? CurrentUser { get; set; }

    public static Settings? CurrentSettings { get; set; }

    public App()
	{
        // Todo: Move this key to a protected location.
        SyncfusionLicenseProvider.RegisterLicense("");
        
        InitializeComponent();

        MainPage = new AppShell();
	}
}
