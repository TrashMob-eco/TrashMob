
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [Route("api/eventtypes")]
    public class EventsTypesController : BaseController
    {
        private readonly IEventTypeRepository eventTypeRepository;

        public EventsTypesController(IEventTypeRepository eventTypeRepository,
                                     TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventTypeRepository = eventTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventTypes(CancellationToken cancellationToken)
        {
            var result = await eventTypeRepository.GetAllEventTypes(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
