namespace TrashMobMobile;

using TrashMob.Models;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    public static User? CurrentUser { get; set; }
}