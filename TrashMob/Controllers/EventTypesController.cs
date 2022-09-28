
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventtypes")]
    public class EventsTypesController : LookupController<EventType>
    {
        public EventsTypesController(TelemetryClient telemetryClient,
                                     IUserRepository userRepository,
                                     ILookupManager<EventType> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
