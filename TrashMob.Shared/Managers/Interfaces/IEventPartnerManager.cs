namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    public interface IEventPartnerManager : IBaseManager<EventPartner>
    {
        Task<IEnumerable<EventPartner>> GetCurrentPartnersAsync(Guid eventId, CancellationToken cancellationToken);
        
        Task<IEnumerable<PartnerLocation>> GetPotentialPartnerLocationsAsync(Guid eventId, CancellationToken cancellationToken);

        Task<IEnumerable<DisplayEventPartner>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
