namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController : KeyedController<ContactRequest>
    {

        public ContactRequestController(TelemetryClient telemetryClient,
                                        IUserRepository userRepository, 
                                        IKeyedManager<ContactRequest> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
