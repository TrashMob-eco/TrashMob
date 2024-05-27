namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerContactManager : IKeyedManager<PartnerContact>
    {
        Task<Partner> GetPartnerForContact(Guid partnerContactId, CancellationToken cancellationToken);
    }
}