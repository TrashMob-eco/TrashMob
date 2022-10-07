namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController : KeyedController<ContactRequest>
    {
        public ContactRequestController(IKeyedManager<ContactRequest> manager)
            : base(manager)
        {
        }
    }
}
