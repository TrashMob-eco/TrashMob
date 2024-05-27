namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/servicetypes")]
    public class ServiceTypesController : LookupController<ServiceType>
    {
        public ServiceTypesController(ILookupManager<ServiceType> manager)
            : base(manager)
        {
        }
    }
}