namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/servicetypes")]
    public class ServiceTypesController(ILookupManager<ServiceType> manager)
        : LookupController<ServiceType>(manager)
    {
    }
}