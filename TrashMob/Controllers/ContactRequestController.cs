namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;

    [Route("api/contactrequest")]
    public class ContactRequestController : BaseController
    {
        private readonly IKeyedManager<ContactRequest> contactRequestManager;

        public ContactRequestController(IKeyedManager<ContactRequest> contactRequestManager, 
                                        TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.contactRequestManager = contactRequestManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddContactRequest(ContactRequest contactRequest)
        {
            await contactRequestManager.Add(contactRequest).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(AddContactRequest));

            return Ok();
        }
    }
}
