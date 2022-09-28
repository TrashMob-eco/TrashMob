
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnerstatuses")]
    public class PartnerStatusesController : LookupController<PartnerStatus>
    {
        public PartnerStatusesController(TelemetryClient telemetryClient,
                                                IUserRepository userRepository,
                                                ILookupManager<PartnerStatus> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
