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
    public class LitterImageManager : KeyedManager<LitterImage>, ILitterImageManager
    {
        private readonly IImageManager imageManager;
        private IDbTransaction dbTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LitterImageManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for litter image data access.</param>
        /// <param name="dbTransaction">The database transaction manager.</param>
        /// <param name="imageManager">The image manager for blob storage operations.</param>
        public LitterImageManager(IKeyedRepository<LitterImage> repository, IDbTransaction dbTransaction,
            IImageManager imageManager) : base(repository)
        {
            this.dbTransaction = dbTransaction;
            this.imageManager = imageManager;
        }

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
            if (instance == null)
            {
                return null;
            }

            return await base.UpdateAsync(instance, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var litterImage = await GetAsync(id, cancellationToken);

            if (litterImage == null)
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
            await imageManager.DeleteImage(id, ImageTypeEnum.LitterImage);

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