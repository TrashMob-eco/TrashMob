namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for partner type lookup operations.
    /// </summary>
    [Route("api/partnertypes")]
    public class PartnerTypesController(ILookupManager<PartnerType> manager)
        : LookupController<PartnerType>(manager)
    {
    }
}