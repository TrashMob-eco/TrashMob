namespace TrashMob.Shared.Managers.Events
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventPartnerStatusManager : LookupManager<EventPartnerStatus>, ILookupManager<EventPartnerStatus>
    {
        public EventPartnerStatusManager(ILookupRepository<EventPartnerStatus> repository) : base(repository)
        {
        }
    }
}
