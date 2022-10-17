
namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartnerlocationstatuses")]
    public class EventPartnerLocationStatusesController : LookupController<EventPartnerLocationStatus>
    {
        public EventPartnerLocationStatusesController(ILookupManager<EventPartnerLocationStatus> manager)
            : base(manager)
        {
        }
    }
}
