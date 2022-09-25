namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/socialmediaaccounttypes")]
    public class SocialMediaAccountTypesController : BaseController
    {
        private readonly ILookupManager<SocialMediaAccountType> manager;

        public SocialMediaAccountTypesController(TelemetryClient telemetryClient,
                                                 IUserRepository userRepository,
                                                 ILookupManager<SocialMediaAccountType> manager)
            : base(telemetryClient, userRepository)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSocialMediaAccountTypes()
        {
            var types = await manager.Get();
            TelemetryClient.TrackEvent(nameof(GetSocialMediaAccountTypes));

            return Ok(types);
        }
    }
}
