
namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
 
    [Route("api/eventstatuses")]
    public class EventsStatusesController : LookupController<EventStatus>
    {
        public EventsStatusesController(ILookupManager<EventStatus> manager)
            : base(manager)
        {
        }
    }
}
