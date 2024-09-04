namespace TrashMob.Shared.Managers.Events
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

    public class EventLitterReportManager : BaseManager<EventLitterReport>, IBaseManager<EventLitterReport>, IEventLitterReportManager
    {
        private readonly IEmailManager emailManager;
        private readonly IKeyedRepository<Event> eventRepository;

        public EventLitterReportManager(IBaseRepository<EventLitterReport> repository, IKeyedRepository<Event> eventRepository,
            IEmailManager emailManager) : base(repository)
        {
            this.eventRepository = eventRepository;
            this.emailManager = emailManager;
        }

        public override async Task<IEnumerable<EventLitterReport>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.LitterReport)
                    .ToListAsync(cancellationToken))
                .AsEnumerable();
        }

        public override async Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            var eventLitterReport = await Repository.Get(ea => ea.EventId == parentId && ea.LitterReportId == secondId)
                .FirstOrDefaultAsync(cancellationToken);

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