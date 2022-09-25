namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController : BaseController
    {
        private readonly IKeyedManager<ContactRequest> contactRequestManager;

        public ContactRequestController(TelemetryClient telemetryClient,
                                        IUserRepository userRepository, 
                                        IKeyedManager<ContactRequest> contactRequestManager)
            : base(telemetryClient, userRepository)
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
