
namespace TrashMob.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Poco;
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Common;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/stats")]
    public class StatsController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly IBaseManager<EventSummary> eventSummaryManager;
        private readonly IEventAttendeeRepository eventAttendeeRepository;

        public StatsController(IEventRepository eventRepository,
                               IBaseManager<EventSummary> eventSummaryManager,
                               IEventAttendeeRepository eventAttendeeRepository)
            : base()
        {
            this.eventRepository = eventRepository;
            this.eventSummaryManager = eventSummaryManager;
            this.eventAttendeeRepository = eventAttendeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            var stats = new Stats();
            var events = eventRepository.GetEvents(cancellationToken);
            stats.TotalEvents = events.Count();

            var eventSummaries = await eventSummaryManager.Get(cancellationToken);
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) + (eventSummaries.Sum(es => es.NumberOfBuckets) / 3);
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes * es.ActualNumberOfAttendees / 60);
            stats.TotalParticipants = eventSummaries.Sum(es => es.ActualNumberOfAttendees);

            return Ok(stats);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStatsByUser(Guid userId, CancellationToken cancellationToken)
        {
            var stats = new Stats();
            var result1 = await eventRepository.GetUserEvents(userId, false, cancellationToken);
            var result2 = await eventAttendeeRepository.GetEventsUserIsAttending(userId, false, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());

            stats.TotalEvents = allResults.Count();
            var eventIds = allResults.Select(e => e.Id);

            var eventSummaries = await eventSummaryManager.Get(es => eventIds.Contains(es.EventId), cancellationToken);
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) + (eventSummaries.Sum(es => es.NumberOfBuckets) / 3);
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes) / 60;

            return Ok(stats);
        }
    }
}
