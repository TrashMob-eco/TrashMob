namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerDocumentManager : IKeyedManager<PartnerDocument>
    {
        Task<Partner> GetPartnerForDocument(Guid partnerDocumentId, CancellationToken cancellationToken);
    }
}