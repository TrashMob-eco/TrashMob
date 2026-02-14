namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for partner request status lookup operations.
    /// </summary>
    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController(ILookupManager<PartnerRequestStatus> manager)
        : LookupController<PartnerRequestStatus>(manager)
    {
    }
}