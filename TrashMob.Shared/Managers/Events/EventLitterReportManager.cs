namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages associations between events and litter reports, tracking which reports are being cleaned by which events.
    /// </summary>
    public class EventLitterReportManager(
        IBaseRepository<EventLitterReport> repository,
        IKeyedRepository<Event> eventRepository,
        IKeyedRepository<LitterReport> litterReportRepository,
        ILogger<EventLitterReportManager> logger)
        : BaseManager<EventLitterReport>(repository), IBaseManager<EventLitterReport>, IEventLitterReportManager
    {

        /// <inheritdoc />
        public override async Task<IEnumerable<EventLitterReport>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.LitterReport)
                    .Include(p => p.LitterReport.LitterImages)
                    .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<EventLitterReport>> GetAsync(Expression<Func<EventLitterReport, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(expression)
                .Include(e => e.Event)
                .Include(e => e.LitterReport)
                .Include(e => e.LitterReport.LitterImages)
                .ToListAsync(cancellationToken);                
        }

        /// <inheritdoc />
        public override async Task<EventLitterReport> AddAsync(EventLitterReport eventLitterReport, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Adding EventLitterReport for EventId {eventLitterReport.EventId} and LitterReportId {eventLitterReport.LitterReportId}");
            
            var litterReport = litterReportRepository.Get(l => l.Id == eventLitterReport.LitterReportId).FirstOrDefault();
            litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Assigned;
            litterReport.LastUpdatedByUserId = userId;
            litterReport.LastUpdatedDate = DateTimeOffset.UtcNow;
            await litterReportRepository.UpdateAsync(litterReport);

            logger.LogInformation("Updated LitterReport Status for LitterReportId {LitterReportId} to {LitterReportStatus}", eventLitterReport.LitterReportId, (int)LitterReportStatusEnum.Assigned);
            eventLitterReport.CreatedByUserId = userId;
            eventLitterReport.CreatedDate = DateTimeOffset.UtcNow;
            eventLitterReport.LastUpdatedByUserId = userId;
            eventLitterReport.LastUpdatedDate = DateTimeOffset.UtcNow;
            return await Repository.AddAsync(eventLitterReport);
        }

        /// <inheritdoc />
        public override async Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Deleting EventLitterReport for EventId {parentId} and LitterReportId {secondId}");
            var eventLitterReport = await Repository.Get(ea => ea.EventId == parentId && ea.LitterReportId == secondId)
                .FirstOrDefaultAsync(cancellationToken);

            var litterReport = litterReportRepository.Get(l => l.Id == eventLitterReport.LitterReportId).FirstOrDefault();
            litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.New;
            await litterReportRepository.UpdateAsync(litterReport);
            logger.LogInformation("Updated LitterReport Status for LitterReportId {LitterReportId} to {LitterReportStatus}", eventLitterReport.LitterReportId, (int)LitterReportStatusEnum.New);

            return await Repository.DeleteAsync(eventLitterReport);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetEventsLitterReportIsAssociatedToAsync(Guid litterReportId, CancellationToken cancellationToken = default)
        {
            var eventLitterReports = await Repository.Get(ea => ea.LitterReportId == litterReportId).ToListAsync(cancellationToken);

            if (eventLitterReports.Any())
            {
                var events = await eventRepository.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled
                                                         && eventLitterReports.Select(ea => ea.EventId).Contains(e.Id))
                    .ToListAsync(cancellationToken);
                return events;
            }

            return new List<Event>();
        }
    }
}