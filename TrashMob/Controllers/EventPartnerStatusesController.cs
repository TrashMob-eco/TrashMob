
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventpartnerstatuses")]
    public class EventPartnerStatusesController : LookupController<EventPartnerStatus>
    {
        public EventPartnerStatusesController(TelemetryClient telemetryClient,
                                              IUserRepository userRepository,
                                              ILookupManager<EventPartnerStatus> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
