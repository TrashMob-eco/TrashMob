namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class NonEventUserNotificationManager : KeyedManager<NonEventUserNotification>, IKeyedManager<NonEventUserNotification>
    {
        public NonEventUserNotificationManager(IKeyedRepository<NonEventUserNotification> repository) : base(repository)
        {
        }
    }
}
