namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class LookupController<T> : BaseController where T : LookupModel
    {
        protected ILookupManager<T> Manager { get; }

        public LookupController(TelemetryClient telemetryClient,
                                IUserRepository userRepository, 
                                ILookupManager<T> manager)
            : base(telemetryClient, userRepository)
        {
            Manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var types = await Manager.Get();
            TelemetryClient.TrackEvent("Get" + nameof(T));

            return Ok(types);
        }
    }
}
