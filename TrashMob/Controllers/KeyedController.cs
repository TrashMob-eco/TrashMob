namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class KeyedController<T> : BaseController where T : KeyedModel
    {
        protected IKeyedManager<T> Manager { get; }

        public KeyedController(TelemetryClient telemetryClient,
                               IUserRepository userRepository, 
                               IKeyedManager<T> manager)
            : base(telemetryClient, userRepository)
        {
            Manager = manager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(T instance)
        {
            await Manager.Add(instance).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Add" + nameof(T));

            return Ok();
        }
    }
}
