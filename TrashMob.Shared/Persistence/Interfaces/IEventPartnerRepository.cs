namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventPartnerRepository
    {
        Task<IEnumerable<EventPartner>> GetEventPartners(Guid eventId, CancellationToken cancellationToken = default);

        Task<IEnumerable<EventPartner>> GetPartnerEvents(Guid partnerId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PartnerLocation>> GetPotentialEventPartners(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventPartner> AddEventPartner(EventPartner eventPartner);

        Task<EventPartner> UpdateEventPartner(EventPartner eventPartner);

        Task<IEnumerable<EventPartner>> GetEventsForPartnerLocation(Guid partnerLocationId, CancellationToken cancellationToken = default);
    }
}
