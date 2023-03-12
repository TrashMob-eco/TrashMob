namespace TrashMobMobileApp;

using Microsoft.Maui.Controls;
using TrashMob.Models;

public partial class App : Application
{
	public static User CurrentUser { get; set; }

    public App(MainPage mainPage)
	{
		InitializeComponent();

        MainPage = new NavigationPage(mainPage);
    }
}
