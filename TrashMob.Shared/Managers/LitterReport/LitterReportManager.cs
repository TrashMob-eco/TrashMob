namespace TrashMob.Shared.Managers.LitterReport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages litter reports including CRUD operations, status tracking, filtering, and email notifications.
    /// </summary>
    public class LitterReportManager(
        IKeyedRepository<LitterReport> repository,
        ILitterImageManager litterImageManager,
        ILogger<LitterReportManager> logger,
        IDbTransaction dbTransaction,
        IEmailManager emailManager,
        IUserManager userManager)
        : KeyedManager<LitterReport>(repository), ILitterReportManager
    {

        /// <inheritdoc />
        public override async Task<LitterReport> UpdateAsync(LitterReport litterReport, Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (litterReport.LitterImages == null || litterReport.LitterImages.Count == 0)
                {
                    return null;
                }

                logger.LogInformation("Updating litter report");

                var existingInstance = Repo.Get().Where(l => l.Id == litterReport.Id)
                    .Include(l => l.LitterImages)
                    .FirstOrDefault();

                if (existingInstance == null)
                {
                    return null;
                }

                // Track if status is changing to Cleaned
                var wasNotCleaned = existingInstance.LitterReportStatusId != (int)LitterReportStatusEnum.Cleaned;
                var isNowCleaned = litterReport.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned;
                var statusChangedToCleaned = wasNotCleaned && isNowCleaned;

                existingInstance.Name = litterReport.Name;
                existingInstance.Description = litterReport.Description;
                existingInstance.LitterReportStatusId = litterReport.LitterReportStatusId;

                foreach (var litterImage in litterReport.LitterImages)
                {
                    if (litterImage.CreatedByUserId == Guid.Empty)
                    {
                        litterImage.LitterReportId = litterReport.Id;
                        litterImage.CreatedByUserId = userId;
                        litterImage.CreatedDate = DateTime.UtcNow;
                        existingInstance.LitterImages.Add(litterImage);
                    }
                }

                List<Guid> deletedIds = [];
                foreach (var litterImage in existingInstance.LitterImages)
                {
                    if (!litterReport.LitterImages.Select(x => x.Id).Contains(litterImage.Id))
                    {
                        deletedIds.Add(litterImage.Id);
                    }
                }

                foreach (var deletedId in deletedIds)
                {
                    await litterImageManager.DeleteAsync(deletedId, userId, cancellationToken);
                    existingInstance.LitterImages.Remove(existingInstance.LitterImages.First(x => x.Id == deletedId));
                }

                var resultLitterReport = await base.UpdateAsync(existingInstance, userId, cancellationToken);

                // Send notification to creator if status changed to Cleaned
                if (statusChangedToCleaned && resultLitterReport != null)
                {
                    await SendLitterReportCleanedNotificationAsync(resultLitterReport, cancellationToken);
                }

                return resultLitterReport;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding litter report");
                return null;
            }
        }

        private async Task SendLitterReportCleanedNotificationAsync(LitterReport litterReport, CancellationToken cancellationToken)
        {
            try
            {
                var creator = await userManager.GetUserByInternalIdAsync(litterReport.CreatedByUserId, cancellationToken);
                if (creator == null || string.IsNullOrWhiteSpace(creator.Email))
                {
                    logger.LogWarning("Could not find creator for litter report {LitterReportId} to send cleaned notification", litterReport.Id);
                    return;
                }

                var firstImage = litterReport.LitterImages?.FirstOrDefault();
                var reportLocation = firstImage?.DisplayAddress() ?? "Unknown location";
                var reportUrl = $"https://www.trashmob.eco/litterreports/{litterReport.Id}";

                var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.LitterReportCleaned.ToString());
                emailCopy = emailCopy.Replace("{ReportName}", litterReport.Name ?? "Untitled Report");
                emailCopy = emailCopy.Replace("{ReportLocation}", reportLocation);
                emailCopy = emailCopy.Replace("{ReportUrl}", reportUrl);

                var subject = "Your litter report has been cleaned!";

                List<EmailAddress> recipients =
                [
                    new() { Name = creator.UserName, Email = creator.Email },
                ];

                var dynamicTemplateData = new
                {
                    username = creator.UserName,
                    emailCopy,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.LitterReportRelated, dynamicTemplateData, recipients,
                        cancellationToken);

                logger.LogInformation("Sent litter report cleaned notification to user {UserId} for report {LitterReportId}", creator.Id, litterReport.Id);
            }
            catch (Exception ex)
            {
                // Don't fail the update operation if email fails
                logger.LogWarning(ex, "Failed to send litter report cleaned notification for report {LitterReportId}", litterReport.Id);
            }
        }

        /// <inheritdoc />
        public override async Task<LitterReport> AddAsync(LitterReport litterReport, Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                if (litterReport.LitterImages == null || litterReport.LitterImages.Count == 0)
                {
                    return null;
                }

                await dbTransaction.BeginTransactionAsync();

                logger.LogInformation("Adding litter report");

                foreach (var litterImage in litterReport.LitterImages)
                {
                    litterImage.LitterReportId = litterReport.Id;
                    litterImage.CreatedByUserId = userId;
                    litterImage.LastUpdatedByUserId = userId;
                    litterImage.CreatedDate = DateTime.UtcNow;
                    litterImage.LastUpdatedDate = DateTime.UtcNow;
                }

                // Add litter report
                var newLitterReport = await base.AddAsync(litterReport, userId, cancellationToken);

                if (newLitterReport == null)
                {
                    return null;
                }

                await dbTransaction.CommitTransactionAsync();

                var message =
                    $"A new litter report: {litterReport.Name} in {litterReport.LitterImages.First().City} has been created on TrashMob.eco!";
                var subject = "New Litter Report Alert";

                List<EmailAddress> recipients =
                [
                    new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
                ];

                var dynamicTemplateData = new
                {
                    username = Constants.TrashMobEmailName,
                    litterReportName = litterReport.Name,
                    eventAddress = litterReport.LitterImages.First().DisplayAddress(),
                    emailCopy = message,
                    subject,
                    googleMapsUrl = litterReport.LitterImages.First().GoogleMapsUrl(),
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.LitterReportEmail,
                        SendGridEmailGroupId.LitterReportRelated, dynamicTemplateData, recipients,
                        CancellationToken.None);

                return newLitterReport;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding litter report");
                await dbTransaction.RollbackTransactionAsync();
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await dbTransaction.BeginTransactionAsync();

                var instance = await Repo.GetAsync(id, cancellationToken);
                if (instance == null)
                {
                    return -1;
                }

                instance.LitterReportStatusId = (int)LitterReportStatusEnum.Cancelled;
                await base.UpdateAsync(instance, userId, cancellationToken);

                var ExistinglitterImages = await litterImageManager.GetByParentIdAsync(id, cancellationToken);
                foreach (var image in ExistinglitterImages)
                {
                    await litterImageManager.DeleteAsync(image.Id, userId, cancellationToken);
                }

                await dbTransaction.CommitTransactionAsync();
                return 1;
            }
            catch
            {
                await dbTransaction.RollbackTransactionAsync();
            }

            return -1;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.New)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Assigned)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetNotCancelledLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cancelled)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.CreatedByUserId == userId)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<LitterReport>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.Get().Include(lr => lr.LitterImages).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<LitterReport> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(lr => lr.Id == id).Include(lr => lr.LitterImages)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Location>> GeLitterLocationsByTimeRangeAsync(DateTimeOffset? startTime,
            DateTimeOffset? endTime, CancellationToken cancellationToken = default)
        {
            var locations = await Repository.Get()
                .Where(lr => lr.LitterImages.Any(li => li.Country != null && li.Region != null && li.City != null) &&
                             (startTime == null || lr.CreatedDate >= startTime) &&
                             (endTime == null || lr.CreatedDate <= endTime))
                .SelectMany(lr => lr.LitterImages)
                .Where(li => li.Country != null && li.Region != null && li.City != null)
                .GroupBy(li => new { li.Country, li.Region, li.City })
                .Select(group => new Location
                    { Country = group.Key.Country, Region = group.Key.Region, City = group.Key.City })
                .ToListAsync(cancellationToken);

            return locations;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetFilteredLitterReportsAsync(LitterReportFilter filter,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => (filter.StartDate == null || lr.CreatedDate >= filter.StartDate) &&
                                        (filter.EndDate == null || lr.CreatedDate <= filter.EndDate) &&
                                        (filter.CreatedByUserId == null ||
                                         lr.CreatedByUserId == filter.CreatedByUserId) &&
                                        (filter.LitterReportStatusId == null ||
                                         lr.LitterReportStatusId == filter.LitterReportStatusId) &&
                                        (filter.Country == null ||
                                         lr.LitterImages.Any(li => li.Country == filter.Country)) &&
                                        (filter.Region == null ||
                                         lr.LitterImages.Any(li => li.Region == filter.Region)) &&
                                        (filter.City == null || lr.LitterImages.Any(li => li.City == filter.City)))
                .Include(lr => lr.LitterImages.Where(f => filter.IncludeLitterImages))
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<LitterReport> UpdateAsync(FullLitterReport instance, Guid userId,
            CancellationToken cancellationToken)
        {
            var existingInstance =
                await Repo.GetWithNoTrackingAsync(instance.Id, cancellationToken);

            if (existingInstance == null)
            {
                return null;
            }

            var updateInstance = await base.UpdateAsync(instance.ToLitterReport(), userId, cancellationToken);

            // Delete images not in the updated report
            var ExistinglitterImages = await litterImageManager.GetByParentIdAsync(instance.Id, cancellationToken);

            foreach (var image in ExistinglitterImages)
            {
                if (!instance.LitterImages.Select(img => img.Id).Contains(image.Id))
                {
                    await litterImageManager.HardDeleteAsync(image.Id, cancellationToken);
                }
            }

            //Add images for the report
            var addingimages = instance.LitterImages.Where(x => x.Id == Guid.Empty).ToList();
            foreach (var image in addingimages)
            {
                image.LitterReportId = updateInstance.Id;
                await litterImageManager.AddAsync(image, userId, cancellationToken);
            }

            return await GetAsync(updateInstance.Id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<LitterReport>> AddWithResultAsync(LitterReport litterReport, Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (litterReport == null)
            {
                return ServiceResult<LitterReport>.Failure("Litter report cannot be null.");
            }

            if (litterReport.LitterImages == null || litterReport.LitterImages.Count == 0)
            {
                return ServiceResult<LitterReport>.Failure("Litter report must include at least one image.");
            }

            if (string.IsNullOrWhiteSpace(litterReport.Name))
            {
                return ServiceResult<LitterReport>.Failure("Litter report name is required.");
            }

            try
            {
                await dbTransaction.BeginTransactionAsync();

                logger.LogInformation("Adding litter report with result");

                foreach (var litterImage in litterReport.LitterImages)
                {
                    litterImage.LitterReportId = litterReport.Id;
                    litterImage.CreatedByUserId = userId;
                    litterImage.LastUpdatedByUserId = userId;
                    litterImage.CreatedDate = DateTime.UtcNow;
                    litterImage.LastUpdatedDate = DateTime.UtcNow;
                }

                // Add litter report
                var newLitterReport = await base.AddAsync(litterReport, userId, cancellationToken);

                if (newLitterReport == null)
                {
                    await dbTransaction.RollbackTransactionAsync();
                    return ServiceResult<LitterReport>.Failure("Failed to save litter report to the database.");
                }

                await dbTransaction.CommitTransactionAsync();

                // Send notification email (non-blocking, don't fail the operation if email fails)
                try
                {
                    var message =
                        $"A new litter report: {litterReport.Name} in {litterReport.LitterImages.First().City} has been created on TrashMob.eco!";
                    var subject = "New Litter Report Alert";

                    List<EmailAddress> recipients =
                    [
                        new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
                    ];

                    var dynamicTemplateData = new
                    {
                        username = Constants.TrashMobEmailName,
                        litterReportName = litterReport.Name,
                        eventAddress = litterReport.LitterImages.First().DisplayAddress(),
                        emailCopy = message,
                        subject,
                        googleMapsUrl = litterReport.LitterImages.First().GoogleMapsUrl(),
                    };

                    await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.LitterReportEmail,
                            SendGridEmailGroupId.LitterReportRelated, dynamicTemplateData, recipients,
                            CancellationToken.None);
                }
                catch (Exception emailEx)
                {
                    // Log but don't fail the operation - the report was created successfully
                    logger.LogWarning(emailEx, "Failed to send notification email for litter report {LitterReportId}", newLitterReport.Id);
                }

                return ServiceResult<LitterReport>.Success(newLitterReport);
            }
            catch (DbUpdateException dbEx)
            {
                logger.LogError(dbEx, "Database error while adding litter report");
                await dbTransaction.RollbackTransactionAsync();
                return ServiceResult<LitterReport>.Failure("A database error occurred while saving the litter report. Please try again.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while adding litter report");
                await dbTransaction.RollbackTransactionAsync();
                return ServiceResult<LitterReport>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public async Task<(int TotalCount, int CleanedCount)> GetLitterReportCountsAsync(CancellationToken cancellationToken = default)
        {
            var totalCount = await Repo.Get().CountAsync(cancellationToken);
            var cleanedCount = await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned)
                .CountAsync(cancellationToken);
            return (totalCount, cleanedCount);
        }

        /// <inheritdoc />
        public async Task<int> GetUserLitterReportCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.CreatedByUserId == userId).CountAsync(cancellationToken);
        }
    }
}