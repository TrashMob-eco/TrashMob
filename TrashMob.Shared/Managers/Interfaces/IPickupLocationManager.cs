namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPickupLocationManager : IKeyedManager<PickupLocation>
    {
        Task SubmitPickupLocations(Guid eventId, Guid userId, CancellationToken cancellationToken);
    }
}
