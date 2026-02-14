namespace TrashMob.Shared.Managers.Teams
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
    /// Manages team photos including CRUD operations and image storage integration.
    /// </summary>
    public class TeamPhotoManager : KeyedManager<TeamPhoto>, ITeamPhotoManager
    {
        private readonly IImageManager imageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamPhotoManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for team photo data access.</param>
        /// <param name="imageManager">The image manager for blob storage operations.</param>
        public TeamPhotoManager(IKeyedRepository<TeamPhoto> repository, IImageManager imageManager) : base(repository)
        {
            this.imageManager = imageManager;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamPhoto>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(p => p.TeamId == teamId)
                .OrderByDescending(p => p.UploadedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<TeamPhoto> AddAsync(TeamPhoto teamPhoto, Guid userId, CancellationToken cancellationToken = default)
        {
            teamPhoto.Id = Guid.NewGuid();
            teamPhoto.UploadedByUserId = userId;
            teamPhoto.UploadedDate = DateTimeOffset.UtcNow;
            teamPhoto.CreatedByUserId = userId;
            teamPhoto.CreatedDate = DateTimeOffset.UtcNow;
            teamPhoto.LastUpdatedByUserId = userId;
            teamPhoto.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await base.AddAsync(teamPhoto, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await base.DeleteAsync(id, cancellationToken);
            await imageManager.DeleteImageAsync(id, ImageTypeEnum.TeamPhoto);

            return 1;
        }
    }
}
