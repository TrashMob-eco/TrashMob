namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface ILitterImageManager : IKeyedManager<LitterImage>
    {
        Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<LitterImage> AddAsync(FullLitterImage instance, Guid userId,
            CancellationToken cancellationToken = default);
    }
}