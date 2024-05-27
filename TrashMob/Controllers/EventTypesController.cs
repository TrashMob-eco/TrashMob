namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventtypes")]
    public class EventsTypesController : LookupController<EventType>
    {
        public EventsTypesController(ILookupManager<EventType> manager)
            : base(manager)
        {
        }
    }
}