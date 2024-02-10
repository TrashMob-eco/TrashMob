namespace TrashMobMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("mainview", typeof(MainView));
        Routing.RegisterRoute("apphost", typeof(AppHost));
    }
}
