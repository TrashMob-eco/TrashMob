namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    public interface ILitterImageManager: IKeyedManager<LitterImage>
    {
        Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<LitterImage> AddAsync(FullLitterImage instance, Guid userId, CancellationToken cancellationToken = default);
    }
}