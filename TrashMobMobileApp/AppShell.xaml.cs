namespace TrashMobMobileApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
	}
}
