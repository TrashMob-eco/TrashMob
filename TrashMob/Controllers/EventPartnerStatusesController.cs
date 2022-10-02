
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartnerstatuses")]
    public class EventPartnerStatusesController : LookupController<EventPartnerStatus>
    {
        public EventPartnerStatusesController(ILookupManager<EventPartnerStatus> manager)
            : base(manager)
        {
        }
    }
}
