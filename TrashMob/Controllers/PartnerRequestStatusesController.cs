namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : LookupController<PartnerRequestStatus>
    {
        public PartnerRequestStatusesController(ILookupManager<PartnerRequestStatus> manager)
            : base(manager)
        {
        }
    }
}