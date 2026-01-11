namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for partner status lookup operations.
    /// </summary>
    [Route("api/partnerstatuses")]
    public class PartnerStatusesController : LookupController<PartnerStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerStatusesController"/> class.
        /// </summary>
        /// <param name="manager">The partner status lookup manager.</param>
        public PartnerStatusesController(ILookupManager<PartnerStatus> manager)
            : base(manager)
        {
        }
    }
}