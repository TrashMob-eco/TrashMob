
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerstatuses")]
    public class PartnerStatusesController : LookupController<PartnerStatus>
    {
        public PartnerStatusesController(TelemetryClient telemetryClient,
                                         ILookupManager<PartnerStatus> manager)
            : base(telemetryClient, manager)
        {
        }
    }
}
