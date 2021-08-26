
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Common;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
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
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags);
            stats.TotalBuckets = eventSummaries.Sum(es => es.NumberOfBuckets);
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes * es.ActualNumberOfAttendees / 60);
            stats.TotalParticipants = eventSummaries.Sum(es => es.ActualNumberOfAttendees);

            return Ok(stats);
        }
    }
}
