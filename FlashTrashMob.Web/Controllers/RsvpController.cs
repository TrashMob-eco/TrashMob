
namespace FlashTrashMob.Web.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using FlashTrashMob.Web.Models;
    using FlashTrashMob.Web.Persistence;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [Authorize]
    public class RsvpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly IMobRepository _repository;

        private readonly UserManager<ApplicationUser> _userManager;

        public RsvpController(IMobRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRsvpAsync(int dinnerId)
        {
            var dinner = await _repository.GetCleanupEventAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            var rsvp = await _repository.CreateRsvpAsync(dinner, user.UserName);
            return new JsonResult(rsvp);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRsvpAsync(int dinnerId)
        {
            var dinner = await _repository.GetCleanupEventAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());

            await _repository.DeleteRsvpAsync(dinner, user.UserName);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}


