
namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
 
    [Route("api/invitationstatuses")]
    public class InvitationStatusesController : LookupController<InvitationStatus>
    {
        public InvitationStatusesController(ILookupManager<InvitationStatus> manager)
            : base(manager)
        {
        }
    }
}
