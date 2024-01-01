namespace TrashMobMobile;

using TrashMob.Models;
using TrashMobMobile.Config;

public partial class App : Application
{
    public static User? CurrentUser { get; set; }

    public static Settings? CurrentSettings { get; set; }

    public App()
	{
		InitializeComponent();

        MainPage = new AppShell();
	}
}
