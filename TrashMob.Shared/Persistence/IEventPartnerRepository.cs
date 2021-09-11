namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventPartnerRepository
    {
        Task<IEnumerable<EventPartner>> GetEventPartners(Guid eventId);

        Task<IEnumerable<EventPartner>> GetPartnerEvents(Guid partnerId);

        Task<IEnumerable<PartnerLocation>> GetPotentialEventPartners(Guid eventId);

        Task<EventPartner> AddEventPartner(EventPartner eventPartner);

        Task<EventPartner> UpdateEventPartner(EventPartner eventPartner);

        Task<IEnumerable<EventPartner>> GetEventsForPartnerLocation(Guid partnerLocationId);
    }
}
