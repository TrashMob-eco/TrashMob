namespace TrashMobMobile;

using TrashMob.Models;
using TrashMobMobile.Services;
using TrashMobMobile.Services.Offline;

public partial class App : Application
{
    private readonly ILoggingService loggingService;

    public App(ILoggingService loggingService, SyncService syncService)
    {
        this.loggingService = loggingService;
        InitializeComponent();

        // Start background sync for offline route/photo/metrics uploads
        syncService.Start();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var exception = (Exception)args.ExceptionObject;
            this.loggingService.LogError(exception);
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            this.loggingService.LogError(args.Exception);
            args.SetObserved();
        };

#pragma warning disable CS0618
        MainPage = new AppShell();
#pragma warning restore CS0618
    }

    public static User? CurrentUser { get; set; }
}