using TrashMob.Models;

namespace TrashMobMobileApp;

public partial class App : Application
{
	public static User CurrentUser { get; set; }

	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}
}
