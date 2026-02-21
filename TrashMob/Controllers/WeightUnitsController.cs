namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing weight unit lookup data.
    /// </summary>
    [Route("api/weightunits")]
    public class WeightUnitsController(ILookupManager<WeightUnit> manager)
        : LookupController<WeightUnit>(manager)
    {
    }
}
