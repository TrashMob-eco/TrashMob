namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventPartnerRepository
    {
        Task<IEnumerable<EventPartner>> GetEventPartners(Guid eventId);

        Task<IEnumerable<PartnerLocation>> GetPotentialEventPartners(Guid eventId);

        Task<int> AddEventPartner(EventPartner eventPartner);

        Task<int> UpdateEventPartner(EventPartner eventPartner);

        Task<IEnumerable<EventPartner>> GetEventsForPartnerLocation(Guid partnerLocationId);
    }
}
