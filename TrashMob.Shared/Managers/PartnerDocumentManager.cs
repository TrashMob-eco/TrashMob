
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerDocumentManager : KeyedManager<PartnerDocument>, IKeyedManager<PartnerDocument>
    {
        public PartnerDocumentManager(IKeyedRepository<PartnerDocument> repository) : base(repository)
        {
        }      
    }
}
