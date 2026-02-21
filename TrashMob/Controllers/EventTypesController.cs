namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for event type lookup operations.
    /// </summary>
    [Route("api/eventtypes")]
    public class EventsTypesController(ILookupManager<EventType> manager)
        : LookupController<EventType>(manager)
    {
    }
}