
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventtypes")]
    public class EventsTypesController : LookupController<EventType>
    {
        public EventsTypesController(TelemetryClient telemetryClient,
                                     ILookupManager<EventType> manager)
            : base(telemetryClient, manager)
        {
        }
    }
}
