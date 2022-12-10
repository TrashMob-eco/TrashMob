
namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
 
    [Route("api/eventstatuses")]
    public class EventStatusesController : LookupController<EventStatus>
    {
        public EventStatusesController(ILookupManager<EventStatus> manager)
            : base(manager)
        {
        }
    }
}
