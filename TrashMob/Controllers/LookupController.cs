namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public abstract class LookupController<T> : BaseController where T : LookupModel
    {
        public LookupController(ILookupManager<T> manager)
        {
            Manager = manager;
        }

        protected ILookupManager<T> Manager { get; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var types = await Manager.GetAsync();
            TelemetryClient.TrackEvent("Get" + nameof(T));

            return Ok(types);
        }
    }
}