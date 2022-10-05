namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public abstract class KeyedController<T> : SecureController where T : KeyedModel
    {
        protected IKeyedManager<T> Manager { get; }

        public KeyedController(IKeyedManager<T> manager) : base()
        {
            Manager = manager;
        }

        [HttpPost]
        [Authorize(Policy = "ValidUser")]
        public virtual async Task<IActionResult> Add(T instance, CancellationToken cancellationToken)
        {
            await Manager.AddAsync(instance, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Add" + nameof(T));

            return Ok();
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            await Manager.GetAsync(cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Get" + nameof(T));

            return Ok();
        }
    }
}
