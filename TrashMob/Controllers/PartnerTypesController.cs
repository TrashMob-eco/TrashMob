namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnertypes")]
    public class PartnerTypesController : LookupController<PartnerType>
    {
        public PartnerTypesController(ILookupManager<PartnerType> manager)
            : base(manager)
        {
        }
    }
}