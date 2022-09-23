namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;

    public interface IUserNotificationManager
    {
        public Task RunAllNotificatons();
    }
}
