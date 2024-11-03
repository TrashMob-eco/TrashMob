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

    public class LitterReportManager : KeyedManager<LitterReport>, ILitterReportManager
    {
        private readonly IDbTransaction dbTransaction;
        private readonly IEmailManager emailManager;
        private readonly ILitterImageManager litterImageManager;
        private readonly ILogger<LitterReportManager> logger;

        public LitterReportManager(IKeyedRepository<LitterReport> repository,
            ILitterImageManager litterImageManager,
            ILogger<LitterReportManager> logger,
            IDbTransaction dbTransaction,
            IEmailManager emailManager) : base(repository)
        {
            this.litterImageManager = litterImageManager;
            this.logger = logger;
            this.dbTransaction = dbTransaction;
            this.emailManager = emailManager;
        }

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

                var deletedIds = new List<Guid>();
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

                return resultLitterReport;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding litter report");
                return null;
            }
        }

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

                var recipients = new List<EmailAddress>
                {
                    new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
                };

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
                        CancellationToken.None)
                    .ConfigureAwait(false);

                return newLitterReport;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding litter report");
                await dbTransaction.RollbackTransactionAsync();
                return null;
            }
        }

        public async Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await dbTransaction.BeginTransactionAsync();

                var instance = await Repo.GetAsync(id, cancellationToken).ConfigureAwait(false);
                if (instance == null)
                {
                    return -1;
                }

                instance.LitterReportStatusId = (int)LitterReportStatusEnum.Cancelled;
                await base.UpdateAsync(instance, userId, cancellationToken).ConfigureAwait(false);

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

        public async Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.New)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Assigned)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetNotCancelledLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cancelled)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.CreatedByUserId == userId)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public override async Task<IEnumerable<LitterReport>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.Get().Include(lr => lr.LitterImages).ToListAsync(cancellationToken);
        }

        public override async Task<LitterReport> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(lr => lr.Id == id).Include(lr => lr.LitterImages)
                .FirstOrDefaultAsync(cancellationToken);
        }

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
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return locations;
        }

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
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<LitterReport> UpdateAsync(FullLitterReport instance, Guid userId,
            CancellationToken cancellationToken)
        {
            var existingInstance =
                await Repo.GetWithNoTrackingAsync(instance.Id, cancellationToken).ConfigureAwait(false);

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
    }
}