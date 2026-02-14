namespace TrashMob.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Admin controller for viewing partner documents across all partners.
    /// All endpoints require site admin privileges.
    /// </summary>
    [Route("api/admin/partnerdocuments")]
    public class PartnerDocumentAdminController(IPartnerDocumentManager manager)
        : SecureController
    {

        /// <summary>
        /// Gets all partner documents across all partners, with partner information included.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all partner documents.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<PartnerDocument>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await manager.GetAllWithPartnerAsync(cancellationToken);
            TrackEvent(nameof(GetAll) + typeof(PartnerDocument));

            return Ok(result);
        }
    }
}
