namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for event type lookup operations.
    /// </summary>
    [Route("api/eventtypes")]
    public class EventsTypesController : LookupController<EventType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsTypesController"/> class.
        /// </summary>
        /// <param name="manager">The event type lookup manager.</param>
        public EventsTypesController(ILookupManager<EventType> manager)
            : base(manager)
        {
        }
    }
}