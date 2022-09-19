
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerNoteManager : KeyedManager<PartnerNote>, IKeyedManager<PartnerNote>
    {
        public PartnerNoteManager(IKeyedRepository<PartnerNote> repository) : base(repository)
        {
        }      
    }
}
