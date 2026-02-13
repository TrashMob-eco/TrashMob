namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController(IKeyedManager<ContactRequest> manager) : BaseController
    {
        /// <summary>
        /// Adds a new contact request.
        /// </summary>
        /// <param name="instance">The contact request instance.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), 400)]
        public virtual async Task<IActionResult> Add([FromBody] ContactRequest instance, CancellationToken cancellationToken)
        {
            await manager.AddAsync(instance, cancellationToken).ConfigureAwait(false);

            TrackEvent("AddContactRequest");

            return Ok();
        }
    }
}
