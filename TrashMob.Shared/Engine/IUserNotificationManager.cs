namespace TrashMob.Shared
{
    using System.Threading.Tasks;

    public interface IUserNotificationManager
    {
        public Task RunAllNotificatons();
    }
}
