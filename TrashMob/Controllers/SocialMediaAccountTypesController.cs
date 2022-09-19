namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;

    [Authorize]
    [Route("api/socialmediaaccounttypes")]
    public class SocialMediaAccountTypesController : BaseController
    {
        private readonly ILookupManager<SocialMediaAccountType> manager;

        public SocialMediaAccountTypesController(ILookupManager<SocialMediaAccountType> manager, 
                                                 TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSocialMediaAccountTypes()
        {
            var types = await manager.Get().ToListAsync();
            TelemetryClient.TrackEvent(nameof(GetSocialMediaAccountTypes));

            return Ok(types);
        }
    }
}
