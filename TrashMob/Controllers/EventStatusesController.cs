
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventstatuses")]
    public class EventsStatusesController : LookupController<EventStatus>
    {
        public EventsStatusesController(TelemetryClient telemetryClient,
                                        IUserRepository userRepository,
                                        ILookupManager<EventStatus> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
