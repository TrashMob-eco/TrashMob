namespace TrashMobMobile;

using TrashMob.Models;

public partial class App : Application
{
    public App()
    {
        // TODO: uncomment if we need syncfusion
        //var sfLicense = Constants.GetSyncfusionKey();

        //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(sfLicense);

        InitializeComponent();

        MainPage = new AppShell();
    }

    public static User? CurrentUser { get; set; }
}