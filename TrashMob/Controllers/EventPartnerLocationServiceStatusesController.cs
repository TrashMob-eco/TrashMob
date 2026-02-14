namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartnerlocationservicestatuses")]
    public class EventPartnerLocationServiceStatusesController(ILookupManager<EventPartnerLocationServiceStatus> manager)
        : LookupController<EventPartnerLocationServiceStatus>(manager)
    {
    }
}