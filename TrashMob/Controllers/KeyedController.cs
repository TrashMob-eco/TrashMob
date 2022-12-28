namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    public abstract class KeyedController<T> : SecureController where T : KeyedModel
    {
        protected IKeyedManager<T> Manager { get; }

        public KeyedController(IKeyedManager<T> manager) : base()
        {
            Manager = manager;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public virtual async Task<IActionResult> Add(T instance, CancellationToken cancellationToken)
        {
            await Manager.AddAsync(instance, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Add" + nameof(T));

            return Ok();
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var results = await Manager.GetAsync(cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Get" + nameof(T));

            return Ok(results);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = Manager.GetAsync(id, cancellationToken);

            var authResult = await AuthorizationService.AuthorizeAsync(User, entity, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var results = await Manager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Delete" + nameof(T));

            return Ok(results);
        }
    }
}
