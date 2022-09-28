
namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnertypes")]
    public class PartnerTypesController : LookupController<PartnerType>
    {
        public PartnerTypesController(TelemetryClient telemetryClient,
                                      IUserRepository userRepository,
                                      ILookupManager<PartnerType> manager)
            : base(telemetryClient, userRepository, manager)
        {
        }
    }
}
