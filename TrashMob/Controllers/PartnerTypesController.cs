
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnertypes")]
    public class PartnerTypesController : LookupController<PartnerType>
    {
        public PartnerTypesController(TelemetryClient telemetryClient,
                                      ILookupManager<PartnerType> manager)
            : base(telemetryClient, manager)
        {
        }
    }
}
