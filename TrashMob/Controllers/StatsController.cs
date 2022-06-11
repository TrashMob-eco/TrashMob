
namespace TrashMob.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;
    using TrashMob.Poco;
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Common;

    [Route("api/stats")]
    public class StatsController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventSummaryRepository eventSummaryRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;

        public StatsController(IEventRepository eventRepository,
                               IEventSummaryRepository eventSummaryRepository,
                               IEventAttendeeRepository eventAttendeeRepository,
                               TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventRepository = eventRepository;
            this.eventSummaryRepository = eventSummaryRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
        }

        [HttpGet]
        public IActionResult GetStats(CancellationToken cancellationToken)
        {
            var stats = new Stats();
            var events = eventRepository.GetEvents(cancellationToken);
            stats.TotalEvents = events.Count();

            var eventSummaries = eventSummaryRepository.GetEventSummaries(cancellationToken);
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

            var eventSummaries = eventSummaryRepository.GetEventSummaries(cancellationToken).Where(es => eventIds.Contains(es.EventId));
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) + (eventSummaries.Sum(es => es.NumberOfBuckets) / 3);
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes / 60);

            return Ok(stats);
        }
    }
}
