namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for partner request status lookup operations.
    /// </summary>
    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : LookupController<PartnerRequestStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerRequestStatusesController"/> class.
        /// </summary>
        /// <param name="manager">The partner request status lookup manager.</param>
        public PartnerRequestStatusesController(ILookupManager<PartnerRequestStatus> manager)
            : base(manager)
        {
        }
    }
}