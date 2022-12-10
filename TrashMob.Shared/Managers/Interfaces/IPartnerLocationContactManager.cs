namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerLocationContactManager : IKeyedManager<PartnerLocationContact>
    {
        Task<Partner> GetPartnerForLocationContact(Guid partnerLocationContactId, CancellationToken cancellationToken);
    }
}
