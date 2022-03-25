
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [Route("api/eventpartnerstatuses")]
    public class EventPartnerStatusesController : BaseController
    {
        private readonly IEventPartnerStatusRepository eventPartnerStatusRepository;

        public EventPartnerStatusesController(IEventPartnerStatusRepository eventPartnerStatusRepository, 
                                              TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventPartnerStatusRepository = eventPartnerStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventPartnerStatuses(CancellationToken cancellationToken)
        {
            var result = await eventPartnerStatusRepository.GetAllEventPartnerStatuses(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
