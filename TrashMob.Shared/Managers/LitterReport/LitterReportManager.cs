namespace TrashMob.Shared.Managers.LitterReport
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Collections.Generic;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class LitterReportManager : KeyedManager<LitterReport>, ILitterReportManager
    {
        private readonly ILitterImageManager litterImageManager;
        private readonly ILogger<LitterReportManager> logger;
        private readonly IDbTransaction dbTransaction;

        public LitterReportManager(IKeyedRepository<LitterReport> repository, 
                                    ILitterImageManager litterImageManager,
                                    ILogger<LitterReportManager> logger,
                                    IDbTransaction dbTransaction) : base(repository)
        {
            this.litterImageManager = litterImageManager;
            this.logger = logger;
            this.dbTransaction = dbTransaction;
        }

        public override async Task<LitterReport> AddAsync(LitterReport litterReport, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await dbTransaction.BeginTransactionAsync();
                
                logger.LogInformation("Adding litter report");
                // Add litter report
                var newLitterReport = await base.AddAsync(litterReport, userId, cancellationToken);

                if (newLitterReport == null)
                {
                    return null;
                }

                //// Add litter images
                //if (litterReport.LitterImages != null && litterReport.LitterImages.Any())
                //{
                //    foreach (var litterImage in litterReport.LitterImages)
                //    {
                //        logger.LogInformation("Adding litter image");

                //        litterImage.LitterReportId = newLitterReport.Id;
                //        LitterImage newLitterImage = await litterImageManager.AddAsync(litterImage, userId, cancellationToken);
                //        newLitterReport.LitterImages.Add(newLitterImage);
                //    }
                //}

                await dbTransaction.CommitTransactionAsync();
                return newLitterReport;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding litter report");
                await dbTransaction.RollbackTransactionAsync();
                return null;
            }
        }

        public async Task<LitterReport> UpdateAsync(FullLitterReport instance, Guid userId, CancellationToken cancellationToken)
        {
            var existingInstance = await Repo.GetWithNoTrackingAsync(instance.Id, cancellationToken).ConfigureAwait(false);

            if (existingInstance == null)
            {
                return null;
            }

            var updateInstance = await base.UpdateAsync(instance.ToLitterReport(), userId, cancellationToken);

            //Delete images not in the updated report
            IEnumerable<LitterImage> ExistinglitterImages = await litterImageManager.GetByParentIdAsync(instance.Id, cancellationToken);

            foreach(LitterImage image in ExistinglitterImages)
            {
                if(!instance.LitterImages.Select(img=>img.Id).Contains(image.Id))
                {
                    await litterImageManager.HardDeleteAsync(image.Id, cancellationToken);
                }
            }

            //Add images for the report
            var addingimages = instance.LitterImages.Where(x=> x.Id == Guid.Empty).ToList();
            foreach (var image in addingimages)
            {
                image.LitterReportId = updateInstance.Id;
                await litterImageManager.AddAsync(image, userId, cancellationToken);
            }

            return await GetAsync(updateInstance.Id, cancellationToken);
        }

        public async Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await dbTransaction.BeginTransactionAsync();

                var instance = await Repo.GetAsync(id, cancellationToken).ConfigureAwait(false);
                if(instance == null)
                {
                    return -1;
                }

                instance.LitterReportStatusId = (int)LitterReportStatusEnum.Cancelled;
                await base.UpdateAsync(instance, userId, cancellationToken).ConfigureAwait(false);

                IEnumerable<LitterImage> ExistinglitterImages = await litterImageManager.GetByParentIdAsync(id, cancellationToken);
                foreach (LitterImage image in ExistinglitterImages)
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

        public async Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.New)
                            .Include(lr => lr.LitterImages)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Assigned)
                            .Include(lr => lr.LitterImages)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned)
                            .Include(lr=>lr.LitterImages)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetNotCancelledLitterReportsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
                            .Include(lr => lr.LitterImages)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cancelled)
                            .Include(lr => lr.LitterImages)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(lr => lr.CreatedByUserId == userId)
                            .Include(lr=>lr.LitterImages)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
        }

        public override async Task<IEnumerable<LitterReport>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.Get().Include(lr=>lr.LitterImages).ToListAsync(cancellationToken);
        }

        public override async Task<LitterReport> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(lr => lr.Id == id).Include(lr => lr.LitterImages).FirstOrDefaultAsync(cancellationToken);
        }
    }
}