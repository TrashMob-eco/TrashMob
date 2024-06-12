namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public abstract class LookupController<T>(ILookupManager<T> manager) : BaseController
        where T : LookupModel
    {
        protected ILookupManager<T> Manager { get; } = manager;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var types = await Manager.GetAsync();
            TelemetryClient.TrackEvent("Get" + nameof(T));

            return Ok(types);
        }
    }
}