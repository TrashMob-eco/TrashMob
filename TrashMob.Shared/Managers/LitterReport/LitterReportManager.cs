namespace TrashMob.Shared.Managers.LitterReport
{
    using System;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Models;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Collections.Generic;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using TrashMob.Shared.Poco;

    public class LitterReportManager : KeyedManager<LitterReport>, ILitterReportManager
    {
        private readonly ILitterImageManager litterImageManager;
        private readonly IDbTransaction dbTransaction;

        public LitterReportManager(IKeyedRepository<LitterReport> repository, 
                                    ILitterImageManager litterImageManager, 
                                    IDbTransaction dbTransaction) : base(repository)
        {
            this.litterImageManager = litterImageManager;
            this.dbTransaction = dbTransaction;
        }

        public async Task<LitterReport> AddAsync(FullLitterReport instance, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await dbTransaction.BeginTransactionAsync();
                //Add litter report
                LitterReport litterReport = instance.ToLitterReport();
                var newLitterReport = await base.AddAsync(litterReport, userId, cancellationToken);

                if(newLitterReport == null)
                {
                    return null;
                }

                //add litter images
                if(instance.LitterImages != null && instance.LitterImages.Any())
                {
                    foreach(FullLitterImage fullLitterImage in instance.LitterImages)
                    {
                        fullLitterImage.LitterReportId = newLitterReport.Id;
                        LitterImage litterImage = await litterImageManager.AddAsync(fullLitterImage, userId, cancellationToken);
                        newLitterReport.LitterImages.Add(litterImage);
                    }
                }

                await dbTransaction.CommitTransactionAsync();
                return newLitterReport;
            }
            catch
            {
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