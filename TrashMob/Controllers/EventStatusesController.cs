
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
 
    [Route("api/eventstatuses")]
    public class EventsStatusesController : LookupController<EventStatus>
    {
        public EventsStatusesController(TelemetryClient telemetryClient,
                                        ILookupManager<EventStatus> manager)
            : base(telemetryClient, manager)
        {
        }
    }
}
