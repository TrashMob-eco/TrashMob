namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared.Poco;

    public interface IEventPartnerLocationManager : IBaseManager<EventPartnerLocation>
    {
        Task<IEnumerable<EventPartnerLocation>> GetCurrentPartnersAsync(Guid eventId, CancellationToken cancellationToken);
        
        Task<IEnumerable<PartnerLocation>> GetPotentialPartnerLocationsAsync(Guid eventId, CancellationToken cancellationToken);

        Task<IEnumerable<DisplayEventPartnerLocation>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);

        Task<IEnumerable<DisplayPartnerLocationEvent>> GetByPartnerLocationIdAsync(Guid partnerId, CancellationToken cancellationToken);
    }
}
