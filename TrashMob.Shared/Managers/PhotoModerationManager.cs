#nullable enable

namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for photo moderation operations.
    /// Handles moderation for LitterImage, TeamPhoto, and EventPhoto entities.
    /// </summary>
    public class PhotoModerationManager : IPhotoModerationManager
    {
        private const string LitterImageType = "LitterImage";
        private const string TeamPhotoType = "TeamPhoto";
        private const string EventPhotoType = "EventPhoto";

        private readonly MobDbContext _dbContext;
        private readonly IEmailManager _emailManager;

        public PhotoModerationManager(MobDbContext dbContext, IEmailManager emailManager)
        {
            _dbContext = dbContext;
            _emailManager = emailManager;
        }

        public async Task<PaginatedList<PhotoModerationItem>> GetPendingPhotosAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var litterImages = await _dbContext.LitterImages
                .Include(i => i.LitterReport)
                .Include(i => i.CreatedByUser)
                .Where(i => i.ModerationStatus == PhotoModerationStatus.Pending && !i.InReview && !i.IsCancelled)
                .Select(i => MapLitterImage(i))
                .ToListAsync(cancellationToken);

            var teamPhotos = await _dbContext.TeamPhotos
                .Include(p => p.Team)
                .Include(p => p.UploadedByUser)
                .Where(p => p.ModerationStatus == PhotoModerationStatus.Pending && !p.InReview)
                .Select(p => MapTeamPhoto(p))
                .ToListAsync(cancellationToken);

            var eventPhotos = await _dbContext.EventPhotos
                .Include(p => p.Event)
                .Include(p => p.UploadedByUser)
                .Where(p => p.ModerationStatus == PhotoModerationStatus.Pending && !p.InReview)
                .Select(p => MapEventPhoto(p))
                .ToListAsync(cancellationToken);

            var allPhotos = litterImages.Concat(teamPhotos).Concat(eventPhotos)
                .OrderByDescending(p => p.UploadedDate)
                .ToList();

            return Paginate(allPhotos, page, pageSize);
        }

        public async Task<PaginatedList<PhotoModerationItem>> GetFlaggedPhotosAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var litterImages = await _dbContext.LitterImages
                .Include(i => i.LitterReport)
                .Include(i => i.CreatedByUser)
                .Include(i => i.ReviewRequestedByUser)
                .Where(i => i.InReview && !i.IsCancelled)
                .Select(i => MapLitterImage(i))
                .ToListAsync(cancellationToken);

            var teamPhotos = await _dbContext.TeamPhotos
                .Include(p => p.Team)
                .Include(p => p.UploadedByUser)
                .Include(p => p.ReviewRequestedByUser)
                .Where(p => p.InReview)
                .Select(p => MapTeamPhoto(p))
                .ToListAsync(cancellationToken);

            var eventPhotos = await _dbContext.EventPhotos
                .Include(p => p.Event)
                .Include(p => p.UploadedByUser)
                .Include(p => p.ReviewRequestedByUser)
                .Where(p => p.InReview)
                .Select(p => MapEventPhoto(p))
                .ToListAsync(cancellationToken);

            var allPhotos = litterImages.Concat(teamPhotos).Concat(eventPhotos)
                .OrderByDescending(p => p.FlaggedDate)
                .ToList();

            return Paginate(allPhotos, page, pageSize);
        }

        public async Task<PaginatedList<PhotoModerationItem>> GetRecentlyModeratedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTimeOffset.UtcNow.AddDays(-30);

            var litterImages = await _dbContext.LitterImages
                .Include(i => i.LitterReport)
                .Include(i => i.CreatedByUser)
                .Include(i => i.ModeratedByUser)
                .Where(i => i.ModerationStatus != PhotoModerationStatus.Pending && i.ModeratedDate >= cutoffDate)
                .Select(i => MapLitterImage(i))
                .ToListAsync(cancellationToken);

            var teamPhotos = await _dbContext.TeamPhotos
                .Include(p => p.Team)
                .Include(p => p.UploadedByUser)
                .Include(p => p.ModeratedByUser)
                .Where(p => p.ModerationStatus != PhotoModerationStatus.Pending && p.ModeratedDate >= cutoffDate)
                .Select(p => MapTeamPhoto(p))
                .ToListAsync(cancellationToken);

            var eventPhotos = await _dbContext.EventPhotos
                .Include(p => p.Event)
                .Include(p => p.UploadedByUser)
                .Include(p => p.ModeratedByUser)
                .Where(p => p.ModerationStatus != PhotoModerationStatus.Pending && p.ModeratedDate >= cutoffDate)
                .Select(p => MapEventPhoto(p))
                .ToListAsync(cancellationToken);

            var allPhotos = litterImages.Concat(teamPhotos).Concat(eventPhotos)
                .OrderByDescending(p => p.ModeratedDate)
                .ToList();

            return Paginate(allPhotos, page, pageSize);
        }

        public async Task<PhotoModerationItem> ApprovePhotoAsync(string photoType, Guid photoId, Guid adminUserId, CancellationToken cancellationToken = default)
        {
            PhotoModerationItem result;

            if (photoType == LitterImageType)
            {
                var image = await _dbContext.LitterImages
                    .Include(i => i.LitterReport)
                    .Include(i => i.CreatedByUser)
                    .FirstOrDefaultAsync(i => i.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"LitterImage {photoId} not found");

                image.ModerationStatus = PhotoModerationStatus.Approved;
                image.InReview = false;
                image.ModeratedByUserId = adminUserId;
                image.ModeratedDate = DateTimeOffset.UtcNow;
                image.LastUpdatedByUserId = adminUserId;
                image.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapLitterImage(image);
            }
            else if (photoType == TeamPhotoType)
            {
                var photo = await _dbContext.TeamPhotos
                    .Include(p => p.Team)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"TeamPhoto {photoId} not found");

                photo.ModerationStatus = PhotoModerationStatus.Approved;
                photo.InReview = false;
                photo.ModeratedByUserId = adminUserId;
                photo.ModeratedDate = DateTimeOffset.UtcNow;
                photo.LastUpdatedByUserId = adminUserId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapTeamPhoto(photo);
            }
            else if (photoType == EventPhotoType)
            {
                var photo = await _dbContext.EventPhotos
                    .Include(p => p.Event)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"EventPhoto {photoId} not found");

                photo.ModerationStatus = PhotoModerationStatus.Approved;
                photo.InReview = false;
                photo.ModeratedByUserId = adminUserId;
                photo.ModeratedDate = DateTimeOffset.UtcNow;
                photo.LastUpdatedByUserId = adminUserId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapEventPhoto(photo);
            }
            else
            {
                throw new ArgumentException($"Unknown photo type: {photoType}");
            }

            // Resolve any pending flags
            await ResolveFlagsAsync(photoType, photoId, adminUserId, "Approved", cancellationToken);

            // Log the action
            await LogModerationActionAsync(photoType, photoId, "Approved", null, adminUserId, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }

        public async Task<PhotoModerationItem> RejectPhotoAsync(string photoType, Guid photoId, string reason, Guid adminUserId, CancellationToken cancellationToken = default)
        {
            PhotoModerationItem result;

            if (photoType == LitterImageType)
            {
                var image = await _dbContext.LitterImages
                    .Include(i => i.LitterReport)
                    .Include(i => i.CreatedByUser)
                    .FirstOrDefaultAsync(i => i.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"LitterImage {photoId} not found");

                image.ModerationStatus = PhotoModerationStatus.Rejected;
                image.InReview = false;
                image.ModeratedByUserId = adminUserId;
                image.ModeratedDate = DateTimeOffset.UtcNow;
                image.ModerationReason = reason;
                image.LastUpdatedByUserId = adminUserId;
                image.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapLitterImage(image);
            }
            else if (photoType == TeamPhotoType)
            {
                var photo = await _dbContext.TeamPhotos
                    .Include(p => p.Team)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"TeamPhoto {photoId} not found");

                photo.ModerationStatus = PhotoModerationStatus.Rejected;
                photo.InReview = false;
                photo.ModeratedByUserId = adminUserId;
                photo.ModeratedDate = DateTimeOffset.UtcNow;
                photo.ModerationReason = reason;
                photo.LastUpdatedByUserId = adminUserId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapTeamPhoto(photo);
            }
            else if (photoType == EventPhotoType)
            {
                var photo = await _dbContext.EventPhotos
                    .Include(p => p.Event)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"EventPhoto {photoId} not found");

                photo.ModerationStatus = PhotoModerationStatus.Rejected;
                photo.InReview = false;
                photo.ModeratedByUserId = adminUserId;
                photo.ModeratedDate = DateTimeOffset.UtcNow;
                photo.ModerationReason = reason;
                photo.LastUpdatedByUserId = adminUserId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapEventPhoto(photo);
            }
            else
            {
                throw new ArgumentException($"Unknown photo type: {photoType}");
            }

            // Resolve any pending flags
            await ResolveFlagsAsync(photoType, photoId, adminUserId, "Rejected", cancellationToken);

            // Log the action
            await LogModerationActionAsync(photoType, photoId, "Rejected", reason, adminUserId, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Send email notification to uploader about photo removal
            await SendPhotoRemovedNotificationAsync(result, reason, cancellationToken);

            return result;
        }

        public async Task<PhotoModerationItem> DismissFlagAsync(string photoType, Guid photoId, Guid adminUserId, CancellationToken cancellationToken = default)
        {
            PhotoModerationItem result;

            if (photoType == LitterImageType)
            {
                var image = await _dbContext.LitterImages
                    .Include(i => i.LitterReport)
                    .Include(i => i.CreatedByUser)
                    .FirstOrDefaultAsync(i => i.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"LitterImage {photoId} not found");

                image.InReview = false;
                image.LastUpdatedByUserId = adminUserId;
                image.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapLitterImage(image);
            }
            else if (photoType == TeamPhotoType)
            {
                var photo = await _dbContext.TeamPhotos
                    .Include(p => p.Team)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"TeamPhoto {photoId} not found");

                photo.InReview = false;
                photo.LastUpdatedByUserId = adminUserId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapTeamPhoto(photo);
            }
            else if (photoType == EventPhotoType)
            {
                var photo = await _dbContext.EventPhotos
                    .Include(p => p.Event)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken)
                    ?? throw new InvalidOperationException($"EventPhoto {photoId} not found");

                photo.InReview = false;
                photo.LastUpdatedByUserId = adminUserId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;

                result = MapEventPhoto(photo);
            }
            else
            {
                throw new ArgumentException($"Unknown photo type: {photoType}");
            }

            // Resolve any pending flags as dismissed
            await ResolveFlagsAsync(photoType, photoId, adminUserId, "Dismissed", cancellationToken);

            // Log the action
            await LogModerationActionAsync(photoType, photoId, "FlagDismissed", null, adminUserId, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }

        public async Task<PhotoFlag> FlagPhotoAsync(string photoType, Guid photoId, string reason, Guid userId, CancellationToken cancellationToken = default)
        {
            // Validate photo exists and set InReview
            if (photoType == LitterImageType)
            {
                var image = await _dbContext.LitterImages.FindAsync(new object[] { photoId }, cancellationToken)
                    ?? throw new InvalidOperationException($"LitterImage {photoId} not found");

                image.InReview = true;
                image.ReviewRequestedByUserId = userId;
                image.ReviewRequestedDate = DateTimeOffset.UtcNow;
                image.LastUpdatedByUserId = userId;
                image.LastUpdatedDate = DateTimeOffset.UtcNow;
            }
            else if (photoType == TeamPhotoType)
            {
                var photo = await _dbContext.TeamPhotos.FindAsync(new object[] { photoId }, cancellationToken)
                    ?? throw new InvalidOperationException($"TeamPhoto {photoId} not found");

                photo.InReview = true;
                photo.ReviewRequestedByUserId = userId;
                photo.ReviewRequestedDate = DateTimeOffset.UtcNow;
                photo.LastUpdatedByUserId = userId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;
            }
            else if (photoType == EventPhotoType)
            {
                var photo = await _dbContext.EventPhotos.FindAsync(new object[] { photoId }, cancellationToken)
                    ?? throw new InvalidOperationException($"EventPhoto {photoId} not found");

                photo.InReview = true;
                photo.ReviewRequestedByUserId = userId;
                photo.ReviewRequestedDate = DateTimeOffset.UtcNow;
                photo.LastUpdatedByUserId = userId;
                photo.LastUpdatedDate = DateTimeOffset.UtcNow;
            }
            else
            {
                throw new ArgumentException($"Unknown photo type: {photoType}");
            }

            // Create the flag record
            var flag = new PhotoFlag
            {
                Id = Guid.NewGuid(),
                PhotoId = photoId,
                PhotoType = photoType,
                FlaggedByUserId = userId,
                FlagReason = reason,
                FlaggedDate = DateTimeOffset.UtcNow,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            _dbContext.PhotoFlags.Add(flag);

            // Log the action
            await LogModerationActionAsync(photoType, photoId, "Flagged", reason, userId, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Send email notification to admins about flagged photo
            await SendPhotoFlaggedNotificationAsync(photoType, photoId, reason, userId, cancellationToken);

            return flag;
        }

        private async Task ResolveFlagsAsync(string photoType, Guid photoId, Guid adminUserId, string resolution, CancellationToken cancellationToken)
        {
            var pendingFlags = await _dbContext.PhotoFlags
                .Where(f => f.PhotoId == photoId && f.PhotoType == photoType && f.ResolvedDate == null)
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

        private async Task LogModerationActionAsync(string photoType, Guid photoId, string action, string? reason, Guid userId, CancellationToken cancellationToken)
        {
            var log = new PhotoModerationLog
            {
                Id = Guid.NewGuid(),
                PhotoId = photoId,
                PhotoType = photoType,
                Action = action,
                Reason = reason,
                PerformedByUserId = userId,
                PerformedDate = DateTimeOffset.UtcNow,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            _dbContext.PhotoModerationLogs.Add(log);
        }

        private async Task SendPhotoRemovedNotificationAsync(PhotoModerationItem photo, string reason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(photo.UploaderEmail))
            {
                return;
            }

            var photoTypeDisplay = photo.PhotoType == LitterImageType ? "Litter Report Image" :
                                   photo.PhotoType == TeamPhotoType ? "Team Photo" : "Event Photo";
            var context = photo.LitterReportName ?? photo.TeamName ?? photo.EventName ?? "Unknown";

            var subject = "Your photo has been removed from TrashMob";

            var message = _emailManager.GetHtmlEmailCopy(NotificationTypeEnum.PhotoRemoved.ToString());
            message = message.Replace("{PhotoType}", photoTypeDisplay);
            message = message.Replace("{Context}", context);
            message = message.Replace("{Reason}", reason);

            var recipients = new List<EmailAddress>
            {
                new() { Name = photo.UploaderName ?? "TrashMob User", Email = photo.UploaderEmail }
            };

            var dynamicTemplateData = new
            {
                username = photo.UploaderName ?? "TrashMob User",
                emailCopy = message,
                subject,
            };

            await _emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General, dynamicTemplateData, recipients, cancellationToken);
        }

        private async Task SendPhotoFlaggedNotificationAsync(string photoType, Guid photoId, string reason, Guid flaggedByUserId, CancellationToken cancellationToken)
        {
            // Get admins to notify
            var admins = await _dbContext.Users
                .Where(u => u.IsSiteAdmin)
                .ToListAsync(cancellationToken);

            if (!admins.Any())
            {
                return;
            }

            // Get photo details for the email
            string uploaderName = "Unknown";
            string uploaderEmail = "Unknown";
            string context = "Unknown";

            if (photoType == LitterImageType)
            {
                var image = await _dbContext.LitterImages
                    .Include(i => i.LitterReport)
                    .Include(i => i.CreatedByUser)
                    .FirstOrDefaultAsync(i => i.Id == photoId, cancellationToken);

                if (image != null)
                {
                    uploaderName = image.CreatedByUser?.UserName ?? "Unknown";
                    uploaderEmail = image.CreatedByUser?.Email ?? "Unknown";
                    context = image.LitterReport?.Name ?? "Litter Report";
                }
            }
            else if (photoType == TeamPhotoType)
            {
                var photo = await _dbContext.TeamPhotos
                    .Include(p => p.Team)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken);

                if (photo != null)
                {
                    uploaderName = photo.UploadedByUser?.UserName ?? "Unknown";
                    uploaderEmail = photo.UploadedByUser?.Email ?? "Unknown";
                    context = photo.Team?.Name ?? "Team";
                }
            }
            else if (photoType == EventPhotoType)
            {
                var photo = await _dbContext.EventPhotos
                    .Include(p => p.Event)
                    .Include(p => p.UploadedByUser)
                    .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken);

                if (photo != null)
                {
                    uploaderName = photo.UploadedByUser?.UserName ?? "Unknown";
                    uploaderEmail = photo.UploadedByUser?.Email ?? "Unknown";
                    context = photo.Event?.Name ?? "Event";
                }
            }

            // Get flagger's name
            var flagger = await _dbContext.Users.FindAsync(new object[] { flaggedByUserId }, cancellationToken);
            var flaggedByName = flagger?.UserName ?? "Unknown";

            var photoTypeDisplay = photoType == LitterImageType ? "Litter Report Image" :
                                   photoType == TeamPhotoType ? "Team Photo" : "Event Photo";
            var subject = "Photo flagged for review on TrashMob";

            var message = _emailManager.GetHtmlEmailCopy(NotificationTypeEnum.PhotoFlagged.ToString());
            message = message.Replace("{PhotoType}", photoTypeDisplay);
            message = message.Replace("{Context}", context);
            message = message.Replace("{UploaderName}", uploaderName);
            message = message.Replace("{UploaderEmail}", uploaderEmail);
            message = message.Replace("{FlagReason}", reason);
            message = message.Replace("{FlaggedByName}", flaggedByName);

            // Send to each admin separately
            foreach (var admin in admins)
            {
                var recipients = new List<EmailAddress>
                {
                    new() { Name = admin.UserName ?? "Admin", Email = admin.Email }
                };

                var dynamicTemplateData = new
                {
                    username = admin.UserName ?? "Admin",
                    emailCopy = message,
                    subject,
                };

                await _emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General, dynamicTemplateData, recipients, cancellationToken);
            }
        }

        private static PhotoModerationItem MapLitterImage(LitterImage image)
        {
            return new PhotoModerationItem
            {
                PhotoId = image.Id,
                PhotoType = LitterImageType,
                ImageUrl = image.AzureBlobURL,
                ModerationStatus = image.ModerationStatus,
                InReview = image.InReview,
                FlaggedDate = image.ReviewRequestedDate,
                FlagReason = null, // Would need to query PhotoFlags for this
                UploadedDate = image.CreatedDate ?? DateTimeOffset.MinValue,
                UploadedByUserId = image.CreatedByUserId,
                UploaderName = image.CreatedByUser?.UserName,
                UploaderEmail = image.CreatedByUser?.Email,
                LitterReportId = image.LitterReportId,
                LitterReportName = image.LitterReport?.Name,
                TeamId = null,
                TeamName = null,
                Caption = null,
                ModeratedDate = image.ModeratedDate,
                ModeratedByName = image.ModeratedByUser?.UserName,
                ModerationReason = image.ModerationReason
            };
        }

        private static PhotoModerationItem MapTeamPhoto(TeamPhoto photo)
        {
            return new PhotoModerationItem
            {
                PhotoId = photo.Id,
                PhotoType = TeamPhotoType,
                ImageUrl = photo.ImageUrl,
                ModerationStatus = photo.ModerationStatus,
                InReview = photo.InReview,
                FlaggedDate = photo.ReviewRequestedDate,
                FlagReason = null, // Would need to query PhotoFlags for this
                UploadedDate = photo.UploadedDate,
                UploadedByUserId = photo.UploadedByUserId,
                UploaderName = photo.UploadedByUser?.UserName,
                UploaderEmail = photo.UploadedByUser?.Email,
                LitterReportId = null,
                LitterReportName = null,
                TeamId = photo.TeamId,
                TeamName = photo.Team?.Name,
                Caption = photo.Caption,
                ModeratedDate = photo.ModeratedDate,
                ModeratedByName = photo.ModeratedByUser?.UserName,
                ModerationReason = photo.ModerationReason
            };
        }

        private static PhotoModerationItem MapEventPhoto(EventPhoto photo)
        {
            return new PhotoModerationItem
            {
                PhotoId = photo.Id,
                PhotoType = EventPhotoType,
                ImageUrl = photo.ImageUrl ?? string.Empty,
                ModerationStatus = photo.ModerationStatus,
                InReview = photo.InReview,
                FlaggedDate = photo.ReviewRequestedDate,
                FlagReason = null, // Would need to query PhotoFlags for this
                UploadedDate = photo.UploadedDate,
                UploadedByUserId = photo.UploadedByUserId,
                UploaderName = photo.UploadedByUser?.UserName,
                UploaderEmail = photo.UploadedByUser?.Email,
                LitterReportId = null,
                LitterReportName = null,
                TeamId = null,
                TeamName = null,
                EventId = photo.EventId,
                EventName = photo.Event?.Name,
                EventPhotoTypeValue = photo.PhotoType,
                Caption = photo.Caption,
                ModeratedDate = photo.ModeratedDate,
                ModeratedByName = photo.ModeratedByUser?.UserName,
                ModerationReason = photo.ModerationReason
            };
        }

        private static PaginatedList<PhotoModerationItem> Paginate(List<PhotoModerationItem> items, int page, int pageSize)
        {
            var totalCount = items.Count;
            var pagedItems = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PhotoModerationItem>(pagedItems, totalCount, page, pageSize);
        }
    }
}
