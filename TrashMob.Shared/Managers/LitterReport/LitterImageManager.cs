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

    public class LitterImageManager : KeyedManager<LitterImage>, ILitterImageManager
    {
        private readonly IImageManager imageManager;
        private IDbTransaction dbTransaction;

        public LitterImageManager(IKeyedRepository<LitterImage> repository, IDbTransaction dbTransaction,
            IImageManager imageManager) : base(repository)
        {
            this.dbTransaction = dbTransaction;
            this.imageManager = imageManager;
        }

        public async Task<LitterImage> AddAsync(FullLitterImage instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var litterImage = instance.ToLitterImage();
            var newLitterImage = await base.AddAsync(litterImage, userId, cancellationToken);
            await UpdateAsync(newLitterImage, userId, cancellationToken);

            return newLitterImage;
        }

        public override async Task<LitterImage> UpdateAsync(LitterImage instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (instance == null)
            {
                return null;
            }

            return await base.UpdateAsync(instance, userId, cancellationToken);
        }

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

        public async Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await base.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            await imageManager.DeleteImage(id, ImageTypeEnum.LitterImage);

            return 1;
        }

        public override async Task<IEnumerable<LitterImage>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.LitterReportId == parentId)
                    .ToListAsync(cancellationToken))
                .AsEnumerable();
        }
    }
}