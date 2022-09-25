namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerDocumentManager : KeyedManager<PartnerDocument>, IKeyedManager<PartnerDocument>
    {
        public PartnerDocumentManager(IKeyedRepository<PartnerDocument> repository) : base(repository)
        {
        }
    }
}
