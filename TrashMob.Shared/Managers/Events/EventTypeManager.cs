namespace TrashMob.Shared.Managers.Events
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventTypeManager : LookupManager<EventType>, ILookupManager<EventType>
    {
        public EventTypeManager(ILookupRepository<EventType> repository) : base(repository)
        {
        }
    }
}
