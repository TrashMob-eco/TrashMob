namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages event photos including CRUD operations and image storage integration.
    /// </summary>
    public class EventPhotoManager(
        IKeyedRepository<EventPhoto> repository,
        IImageManager imageManager,
        MobDbContext dbContext)
        : KeyedManager<EventPhoto>(repository), IEventPhotoManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<EventPhoto>> GetByEventIdAsync(
            Guid eventId,
            bool includeModerated = false,
            CancellationToken cancellationToken = default)
        {
            var query = Repository.Get()
                .Where(p => p.EventId == eventId);

            if (!includeModerated)
            {
                query = query.Where(p =>
                    p.ModerationStatus != PhotoModerationStatus.Rejected &&
                    !p.InReview);
            }

            return await query
                .OrderBy(p => p.PhotoType)
                .ThenByDescending(p => p.UploadedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventPhoto>> GetByEventIdAndTypeAsync(
            Guid eventId,
            EventPhotoType photoType,
            CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(p => p.EventId == eventId &&
                           p.PhotoType == photoType &&
                           p.ModerationStatus != PhotoModerationStatus.Rejected &&
                           !p.InReview)
                .OrderByDescending(p => p.UploadedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventPhoto> AddPhotoAsync(
            Guid eventId,
            EventPhotoType photoType,
            string caption,
            string imageData,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var photo = new EventPhoto
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                PhotoType = photoType,
                Caption = caption,
                UploadedByUserId = userId,
                UploadedDate = DateTimeOffset.UtcNow,
                ModerationStatus = PhotoModerationStatus.Pending
            };

            // Upload image to blob storage
            if (!string.IsNullOrWhiteSpace(imageData))
            {
                // Convert base64 to IFormFile-like stream
                var bytes = Convert.FromBase64String(imageData);
                using var stream = new MemoryStream(bytes);

                var imageUpload = new ImageUpload
                {
                    ParentId = photo.Id,
                    ImageType = ImageTypeEnum.EventPhoto,
                    FormFile = new FormFileWrapper(stream, "eventphoto.jpg")
                };

                await imageManager.UploadImageAsync(imageUpload);

                // Get the URLs
                photo.ImageUrl = await imageManager.GetImageUrlAsync(
                    photo.Id, ImageTypeEnum.EventPhoto, ImageSizeEnum.Raw, cancellationToken);
                photo.ThumbnailUrl = await imageManager.GetImageUrlAsync(
                    photo.Id, ImageTypeEnum.EventPhoto, ImageSizeEnum.Thumb, cancellationToken);
            }

            return await AddAsync(photo, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventPhoto> UpdatePhotoMetadataAsync(
            Guid photoId,
            string caption,
            EventPhotoType photoType,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var photo = await GetAsync(photoId, cancellationToken);
            if (photo is null)
            {
                return null;
            }

            photo.Caption = caption;
            photo.PhotoType = photoType;

            return await UpdateAsync(photo, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> DeletePhotoAsync(
            Guid photoId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var photo = await GetAsync(photoId, cancellationToken);
            if (photo is null)
            {
                return false;
            }

            // Soft delete - mark as rejected
            photo.ModerationStatus = PhotoModerationStatus.Rejected;
            photo.ModeratedByUserId = userId;
            photo.ModeratedDate = DateTimeOffset.UtcNow;
            photo.ModerationReason = "Deleted by user";

            await UpdateAsync(photo, userId, cancellationToken);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> FlagPhotoAsync(
            Guid photoId,
            Guid userId,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var photo = await GetAsync(photoId, cancellationToken);
            if (photo is null)
            {
                return false;
            }

            // Update photo to show it's under review
            photo.InReview = true;
            photo.ReviewRequestedByUserId = userId;
            photo.ReviewRequestedDate = DateTimeOffset.UtcNow;

            await UpdateAsync(photo, userId, cancellationToken);

            // Create the flag record
            var flag = new PhotoFlag
            {
                Id = Guid.NewGuid(),
                PhotoId = photoId,
                PhotoType = "EventPhoto",
                FlaggedByUserId = userId,
                FlagReason = reason,
                FlaggedDate = DateTimeOffset.UtcNow,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            dbContext.PhotoFlags.Add(flag);

            // Log the action
            await LogModerationActionAsync(photoId, "Flagged", reason, userId, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventPhoto>> GetPendingModerationAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(p => p.InReview || p.ModerationStatus == PhotoModerationStatus.Pending)
                .Include(p => p.Event)
                .Include(p => p.UploadedByUser)
                .Include(p => p.ReviewRequestedByUser)
                .OrderBy(p => p.ReviewRequestedDate ?? p.UploadedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventPhoto> ApprovePhotoAsync(
            Guid photoId,
            Guid moderatorId,
            CancellationToken cancellationToken = default)
        {
            var photo = await GetAsync(photoId, cancellationToken);
            if (photo is null)
            {
                return null;
            }

            photo.ModerationStatus = PhotoModerationStatus.Approved;
            photo.InReview = false;
            photo.ModeratedByUserId = moderatorId;
            photo.ModeratedDate = DateTimeOffset.UtcNow;

            // Resolve any pending flags
            await ResolveFlagsAsync(photoId, moderatorId, "Approved", cancellationToken);

            // Log the moderation action
            await LogModerationActionAsync(photoId, "Approved", null, moderatorId, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return await UpdateAsync(photo, moderatorId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventPhoto> RejectPhotoAsync(
            Guid photoId,
            Guid moderatorId,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var photo = await GetAsync(photoId, cancellationToken);
            if (photo is null)
            {
                return null;
            }

            photo.ModerationStatus = PhotoModerationStatus.Rejected;
            photo.InReview = false;
            photo.ModeratedByUserId = moderatorId;
            photo.ModeratedDate = DateTimeOffset.UtcNow;
            photo.ModerationReason = reason;

            // Resolve any pending flags
            await ResolveFlagsAsync(photoId, moderatorId, PhotoModerationAction.Rejected, cancellationToken);

            // Log the moderation action
            await LogModerationActionAsync(photoId, PhotoModerationAction.Rejected, reason, moderatorId, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return await UpdateAsync(photo, moderatorId, cancellationToken);
        }

        private async Task ResolveFlagsAsync(Guid photoId, Guid adminUserId, string resolution, CancellationToken cancellationToken)
        {
            var pendingFlags = await dbContext.PhotoFlags
                .Where(f => f.PhotoId == photoId && f.PhotoType == "EventPhoto" && f.ResolvedDate == null)
                .ToListAsync(cancellationToken);

            foreach (var flag in pendingFlags)
            {
                flag.ResolvedDate = DateTimeOffset.UtcNow;
                flag.ResolvedByUserId = adminUserId;
                flag.Resolution = resolution;
                flag.LastUpdatedByUserId = adminUserId;
                flag.LastUpdatedDate = DateTimeOffset.UtcNow;
            }
        }

        private Task LogModerationActionAsync(Guid photoId, string action, string reason, Guid userId, CancellationToken cancellationToken)
        {
            var log = new PhotoModerationLog
            {
                Id = Guid.NewGuid(),
                PhotoId = photoId,
                PhotoType = "EventPhoto",
                Action = action,
                Reason = reason,
                PerformedByUserId = userId,
                PerformedDate = DateTimeOffset.UtcNow,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            dbContext.PhotoModerationLogs.Add(log);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<EventPhoto>> GetByParentIdAsync(
            Guid parentId,
            CancellationToken cancellationToken)
        {
            return await GetByEventIdAsync(parentId, false, cancellationToken);
        }
    }

    /// <summary>
    /// Simple wrapper to convert a stream to IFormFile for image upload.
    /// </summary>
    internal class FormFileWrapper : IFormFile
    {
        private readonly MemoryStream stream;
        private readonly string fileName;

        public FormFileWrapper(MemoryStream stream, string fileName)
        {
            // Create a copy of the stream that stays open
            this.stream = new MemoryStream();
            stream.Position = 0;
            stream.CopyTo(this.stream);
            this.stream.Position = 0;
            this.fileName = fileName;
        }

        public string ContentType => "image/jpeg";
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{fileName}\"";
        public IHeaderDictionary Headers => null;
        public long Length => stream.Length;
        public string Name => "file";
        public string FileName => fileName;

        public void CopyTo(Stream target) => stream.CopyTo(target);
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
            => await stream.CopyToAsync(target, cancellationToken);
        public Stream OpenReadStream()
        {
            stream.Position = 0;
            return stream;
        }
    }
}
