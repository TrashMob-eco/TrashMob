namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for invitation status lookup operations.
    /// </summary>
    [Route("api/invitationstatuses")]
    public class InvitationStatusesController(ILookupManager<InvitationStatus> manager)
        : LookupController<InvitationStatus>(manager)
    {
    }
}