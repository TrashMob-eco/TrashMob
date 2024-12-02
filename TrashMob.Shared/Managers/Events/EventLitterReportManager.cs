namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventLitterReportManager : BaseManager<EventLitterReport>, IBaseManager<EventLitterReport>, IEventLitterReportManager
    {
        private readonly IEmailManager emailManager;
        private readonly ILogger<EventLitterReportManager> logger;
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IKeyedRepository<LitterReport> litterReportRepository;

        public EventLitterReportManager(IBaseRepository<EventLitterReport> repository, IKeyedRepository<Event> eventRepository, IKeyedRepository<LitterReport> litterReportRepository,
            IEmailManager emailManager, ILogger<EventLitterReportManager> logger) : base(repository)
        {
            this.eventRepository = eventRepository;
            this.litterReportRepository = litterReportRepository;
            this.emailManager = emailManager;
            this.logger = logger;
        }

        public override async Task<IEnumerable<EventLitterReport>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.LitterReport)
                    .Include(p => p.LitterReport.LitterImages)
                    .ToListAsync(cancellationToken))
                .AsEnumerable();
        }

        public override async Task<EventLitterReport> AddAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Adding EventLitterReport for EventId {eventLitterReport.EventId} and LitterReportId {eventLitterReport.LitterReportId}");
            var litterReport = litterReportRepository.Get(l => l.Id == eventLitterReport.LitterReportId).FirstOrDefault();
            litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Assigned;
            await litterReportRepository.UpdateAsync(litterReport);
            logger.LogInformation("Updated LitterReport Status for LitterReportId {LitterReportId} to {LitterReportStatus}", eventLitterReport.LitterReportId, (int)LitterReportStatusEnum.Assigned);

            return await Repository.AddAsync(eventLitterReport);
        }

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