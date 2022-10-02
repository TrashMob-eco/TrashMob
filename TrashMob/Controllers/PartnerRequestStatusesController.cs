
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : LookupController<PartnerRequestStatus>
    {
        public PartnerRequestStatusesController(TelemetryClient telemetryClient,
                                                ILookupManager<PartnerRequestStatus> manager)
            : base(telemetryClient, manager)
        {
        }
    }
}
