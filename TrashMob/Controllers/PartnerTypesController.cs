namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for partner type lookup operations.
    /// </summary>
    [Route("api/partnertypes")]
    public class PartnerTypesController : LookupController<PartnerType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerTypesController"/> class.
        /// </summary>
        /// <param name="manager">The partner type lookup manager.</param>
        public PartnerTypesController(ILookupManager<PartnerType> manager)
            : base(manager)
        {
        }
    }
}