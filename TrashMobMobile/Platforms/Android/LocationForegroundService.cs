namespace TrashMobMobile;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

[Service(ForegroundServiceType = ForegroundService.TypeLocation, Exported = false)]
public class LocationForegroundService : Service
{
    private const int NotificationId = 9001;
    private const string ChannelId = "trashmob_location";

    public override IBinder? OnBind(Intent? intent) => null;

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        CreateNotificationChannel();

        var notification = new Notification.Builder(this, ChannelId)
            .SetContentTitle("TrashMob")
            .SetContentText("Recording your cleanup route...")
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetOngoing(true)
            .Build();

        StartForeground(NotificationId, notification, ForegroundService.TypeLocation);

        return StartCommandResult.Sticky;
    }

    private void CreateNotificationChannel()
    {
        var channel = new NotificationChannel(ChannelId, "Route Tracking", NotificationImportance.Low)
        {
            Description = "Shows while TrashMob is recording your cleanup route"
        };

        var notificationManager = GetSystemService(NotificationService) as NotificationManager;
        notificationManager?.CreateNotificationChannel(channel);
    }
}
