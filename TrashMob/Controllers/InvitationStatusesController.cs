namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for invitation status lookup operations.
    /// </summary>
    [Route("api/invitationstatuses")]
    public class InvitationStatusesController : LookupController<InvitationStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvitationStatusesController"/> class.
        /// </summary>
        /// <param name="manager">The invitation status lookup manager.</param>
        public InvitationStatusesController(ILookupManager<InvitationStatus> manager)
            : base(manager)
        {
        }
    }
}