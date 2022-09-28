namespace TrashMob.Shared.Managers.Events
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventStatusManager : LookupManager<EventStatus>, ILookupManager<EventStatus>
    {
        public EventStatusManager(ILookupRepository<EventStatus> repository) : base(repository)
        {
        }
    }
}
