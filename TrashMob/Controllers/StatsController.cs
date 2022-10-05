
namespace TrashMob.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Poco;
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Common;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/stats")]
    public class StatsController : BaseController
    {
        private readonly IEventManager eventManager;
        private readonly IBaseManager<EventSummary> eventSummaryManager;
        private readonly IEventAttendeeManager eventAttendeeManager;

        public StatsController(IEventManager eventManager,
                               IBaseManager<EventSummary> eventSummaryManager,
                               IEventAttendeeManager eventAttendeeManager)
            : base()
        {
            this.eventManager = eventManager;
            this.eventSummaryManager = eventSummaryManager;
            this.eventAttendeeManager = eventAttendeeManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            var stats = new Stats();
            var events = await eventManager.Get(cancellationToken);
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
            var result1 = await eventManager.GetUserEvents(userId, false, cancellationToken);
            var result2 = await eventAttendeeManager.GetEventsUserIsAttending(userId, false, cancellationToken).ConfigureAwait(false);

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
