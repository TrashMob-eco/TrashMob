namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/socialmediaaccounttypes")]
    public class SocialMediaAccountTypesController : LookupController<SocialMediaAccountType>
    {
        public SocialMediaAccountTypesController(ILookupManager<SocialMediaAccountType> manager)
            : base(manager)
        {
        }
    }
}