namespace TrashMobMobile;

using TrashMob.Models;

public partial class App : Application
{
    public static User? CurrentUser { get; set; }

    public App()
	{
        var sfLicense = Constants.GetSyncfusionKey();

        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(sfLicense);

        InitializeComponent();

        MainPage = new AppShell();
	}
}
