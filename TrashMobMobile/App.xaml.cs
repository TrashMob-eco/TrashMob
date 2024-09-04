namespace TrashMobMobile;

using TrashMob.Models;
using TrashMobMobile.Services;

public partial class App : Application
{
    private readonly ILoggingService loggingService;

    public App(ILoggingService loggingService)
    {
        this.loggingService = loggingService;
        InitializeComponent();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var exception = (Exception)args.ExceptionObject;
            this.loggingService.LogError(exception);
        };

        AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
        {
            this.loggingService.LogError(args.Exception);
        };

        MainPage = new AppShell();
    }

    public static User? CurrentUser { get; set; }
}