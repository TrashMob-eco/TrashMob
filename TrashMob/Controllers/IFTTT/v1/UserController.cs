namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;

    [Route("api/ifttt/v1/[controller]")]
    [ApiController]
    public class UserController : SecureController
    {
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public ActionResult Index()
        {
            var 
            
        }
    }
}
