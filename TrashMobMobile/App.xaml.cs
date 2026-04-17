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

    /// <summary>
    /// Override to return the existing window when the app is already running,
    /// preventing "already associated with an active Activity" crashes (#3266).
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (Windows.Count > 0)
        {
            return Windows[0];
        }

        return base.CreateWindow(activationState);
    }
}