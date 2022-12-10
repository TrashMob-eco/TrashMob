namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController : BaseController
    {
        private readonly IKeyedManager<ContactRequest> manager;

        public ContactRequestController(IKeyedManager<ContactRequest> manager)
        {
            this.manager = manager;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Add(ContactRequest instance, CancellationToken cancellationToken)
        {
            await manager.AddAsync(instance, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("AddContactRequest");

            return Ok();
        }
    }
}
