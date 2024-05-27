﻿namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventPartnerLocationServiceManager : IBaseManager<EventPartnerLocationService>
    {
        Task<IEnumerable<EventPartnerLocationService>> GetCurrentPartnersAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<PartnerLocation>> GetPotentialPartnerLocationsAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayEventPartnerLocationService>> GetByEventAndPartnerLocationAsync(Guid eventId,
            Guid partnerLocationId, CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayEventPartnerLocation>> GetByEventAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayPartnerLocationServiceEvent>> GetByPartnerLocationAsync(Guid partnerLocationId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayPartnerLocationServiceEvent>> GetByUserAsync(Guid userId,
            CancellationToken cancellationToken = default);

        Task<PartnerLocation> GetHaulingPartnerLocationForEvent(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(Guid eventId, Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken = default);
    }
}