
namespace TrashMob.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;
    using TrashMob.Poco;

    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventSummaryRepository eventSummaryRepository;

        public StatsController(IEventRepository eventRepository, IEventSummaryRepository eventSummaryRepository)
        {
            this.eventRepository = eventRepository;
            this.eventSummaryRepository = eventSummaryRepository;
        }

        [HttpGet]
        public IActionResult GetStats()
        {
            var stats = new Stats();
            var events = eventRepository.GetEvents();
            stats.TotalEvents = events.Count();

            var eventSummaries = eventSummaryRepository.GetEventSummaries();
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) + (eventSummaries.Sum(es => es.NumberOfBuckets) / 3);
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes * es.ActualNumberOfAttendees / 60);
            stats.TotalParticipants = eventSummaries.Sum(es => es.ActualNumberOfAttendees);

            return Ok(stats);
        }
    }
}
