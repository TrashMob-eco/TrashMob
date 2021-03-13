
namespace FlashTrashMob.Web.Controllers
{
    using System.Net;
    using System.Security.Claims;
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
        public async Task<IActionResult> CreateRsvpAsync(int cleanupEventId)
        {
            var cleanupEvent = await _repository.GetCleanupEventAsync(cleanupEventId);
            if (cleanupEvent == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var rsvp = await _repository.CreateRsvpAsync(cleanupEvent, user.UserName);
            return new JsonResult(rsvp);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRsvpAsync(int cleanupEventId)
        {
            var cleanupEvent = await _repository.GetCleanupEventAsync(cleanupEventId);
            if (cleanupEvent == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _repository.DeleteRsvpAsync(cleanupEvent, user.UserName);
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}


