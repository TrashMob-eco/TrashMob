namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerNoteManager : KeyedManager<PartnerNote>, IKeyedManager<PartnerNote>
    {
        public PartnerNoteManager(IKeyedRepository<PartnerNote> repository) : base(repository)
        {
        }
    }
}
