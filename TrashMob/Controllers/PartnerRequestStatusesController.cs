
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : LookupController<PartnerRequestStatus>
    {
        public PartnerRequestStatusesController(TelemetryClient telemetryClient,
                                                IUserRepository userRepository,
                                                ILookupManager<PartnerRequestStatus> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
