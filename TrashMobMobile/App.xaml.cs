using TrashMob.Models;
using TrashMobMobile.Config;

namespace TrashMobMobile;
public partial class App : Application
{
    public static User? CurrentUser { get; set; }

    public App()
    {
        //Register Syncfusion license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Variables.SYNCFUSION_API_LICENSE_KEY);
        InitializeComponent();

        MainPage = new AppShell();
    }
}
