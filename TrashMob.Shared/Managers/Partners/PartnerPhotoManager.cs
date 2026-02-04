namespace TrashMob.Shared.Managers.Partners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for partner/community photo operations.
    /// </summary>
    public class PartnerPhotoManager : KeyedManager<PartnerPhoto>, IPartnerPhotoManager
    {
        private readonly IImageManager imageManager;

        public PartnerPhotoManager(
            IKeyedRepository<PartnerPhoto> repository,
            IImageManager imageManager)
            : base(repository)
        {
            this.imageManager = imageManager;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PartnerPhoto>> GetByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(p => p.PartnerId == partnerId)
                .OrderByDescending(p => p.UploadedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<PartnerPhoto> AddAsync(PartnerPhoto partnerPhoto, Guid userId, CancellationToken cancellationToken = default)
        {
            partnerPhoto.Id = Guid.NewGuid();
            partnerPhoto.UploadedByUserId = userId;
            partnerPhoto.UploadedDate = DateTimeOffset.UtcNow;
            partnerPhoto.ModerationStatus = PhotoModerationStatus.Approved; // Auto-approve for now
            partnerPhoto.CreatedByUserId = userId;
            partnerPhoto.CreatedDate = DateTimeOffset.UtcNow;
            partnerPhoto.LastUpdatedByUserId = userId;
            partnerPhoto.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await base.AddAsync(partnerPhoto, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Delete from database first
            await base.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            // Then delete from blob storage
            await imageManager.DeleteImage(id, ImageTypeEnum.PartnerPhoto);

            return 1;
        }
    }
}
