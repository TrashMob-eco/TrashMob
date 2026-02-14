namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for partner status lookup operations.
    /// </summary>
    [Route("api/partnerstatuses")]
    public class PartnerStatusesController(ILookupManager<PartnerStatus> manager)
        : LookupController<PartnerStatus>(manager)
    {
    }
}