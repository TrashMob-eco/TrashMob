namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Abstract base controller for lookup endpoints.
    /// </summary>
    /// <typeparam name="T">The type of lookup model.</typeparam>
    public abstract class LookupController<T>(ILookupManager<T> manager) : BaseController
        where T : LookupModel
    {
        protected ILookupManager<T> Manager { get; } = manager;

        /// <summary>
        /// Gets all items of the lookup type.
        /// </summary>
        /// <remarks>A list of lookup items.</remarks>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var types = await Manager.GetAsync();
            TelemetryClient.TrackEvent("Get" + nameof(T));

            return Ok(types);
        }
    }
}