﻿namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerLocationManager : IKeyedManager<PartnerLocation>
    {
        Task<Partner> GetPartnerForLocationAsync(Guid partnerLocationId, CancellationToken cancellationToken);
    }
}