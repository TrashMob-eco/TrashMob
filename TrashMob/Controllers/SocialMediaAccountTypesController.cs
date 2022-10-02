namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/socialmediaaccounttypes")]
    public class SocialMediaAccountTypesController : LookupController<SocialMediaAccountType>
    {
        public SocialMediaAccountTypesController(TelemetryClient telemetryClient,
                                                 ILookupManager<SocialMediaAccountType> manager)
            : base(telemetryClient, manager)
        {
        }
    }
}
