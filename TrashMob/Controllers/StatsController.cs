
namespace TrashMob.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;
    using TrashMob.Poco;
    using System.Threading;
    using Microsoft.ApplicationInsights;

    [Route("api/stats")]
    public class StatsController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventSummaryRepository eventSummaryRepository;

        public StatsController(IEventRepository eventRepository,
                               IEventSummaryRepository eventSummaryRepository,
                               TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventRepository = eventRepository;
            this.eventSummaryRepository = eventSummaryRepository;
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
    }
}
