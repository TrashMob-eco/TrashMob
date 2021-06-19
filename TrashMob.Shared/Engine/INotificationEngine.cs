namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;

    public interface INotificationEngine
    {
        public Task GenerateNotificationsAsync();
    }
}
