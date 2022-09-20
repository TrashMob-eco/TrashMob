
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventstatuses")]
    public class EventsStatusesController : BaseController
    {
        private readonly IEventStatusRepository eventStatusRepository;

        public EventsStatusesController(IEventStatusRepository eventStatusRepository,
                                        TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventStatusRepository = eventStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStatuses(CancellationToken cancellationToken)
        {
            var result = await eventStatusRepository.GetAllEventStatuses(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
