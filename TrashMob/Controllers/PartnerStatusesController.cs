
namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerstatuses")]
    public class PartnerStatusesController : LookupController<PartnerStatus>
    {
        public PartnerStatusesController(ILookupManager<PartnerStatus> manager)
            : base(manager)
        {
        }
    }
}
