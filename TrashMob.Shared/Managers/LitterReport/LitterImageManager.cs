namespace TrashMob.Shared.Managers.LitterReport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages litter images including CRUD operations and image storage integration.
    /// </summary>
    public class LitterImageManager(
        IKeyedRepository<LitterImage> repository,
        IImageManager imageManager)
        : KeyedManager<LitterImage>(repository), ILitterImageManager
    {

        /// <inheritdoc />
        public async Task<LitterImage> AddAsync(FullLitterImage instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var litterImage = instance.ToLitterImage();
            var newLitterImage = await base.AddAsync(litterImage, userId, cancellationToken);
            await UpdateAsync(newLitterImage, userId, cancellationToken);

            return newLitterImage;
        }

        /// <inheritdoc />
        public override async Task<LitterImage> UpdateAsync(LitterImage instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (instance is null)
            {
                return null;
            }

            return await base.UpdateAsync(instance, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var litterImage = await GetAsync(id, cancellationToken);

            if (litterImage is null)
            {
                return -1;
            }

            litterImage.IsCancelled = true;
            await UpdateAsync(litterImage, userId, cancellationToken);

            return 1;
        }

        /// <inheritdoc />
        public async Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await base.DeleteAsync(id, cancellationToken);
            await imageManager.DeleteImageAsync(id, ImageTypeEnum.LitterImage);

            return 1;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<LitterImage>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.LitterReportId == parentId)
                    .ToListAsync(cancellationToken);
        }
    }
}