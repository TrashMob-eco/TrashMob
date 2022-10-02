namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController : KeyedController<ContactRequest>
    {

        public ContactRequestController(TelemetryClient telemetryClient,
                                        IAuthorizationService authorizationService,
                                        IKeyedManager<ContactRequest> manager)
            : base(telemetryClient, authorizationService, manager)
        {
        }
    }
}
