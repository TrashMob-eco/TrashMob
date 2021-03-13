namespace FlashTrashMob.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using FlashTrashMob.Web.Common;
    using FlashTrashMob.Web.Models;
    using FlashTrashMob.Web.Persistence;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [Authorize]
    public class CleanupEventController : Controller
    {
        private readonly IMobRepository _repository;

        private readonly UserManager<ApplicationUser> _userManager;

        public CleanupEventController(IMobRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet("{id:int}", Name = "GetCleanupEventById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCleanupEventAsync(int id)
        {
            var dinner = await _repository.GetCleanupEventAsync(id);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            return new ObjectResult(dinner);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<CleanupEvent>> GetCleanupEventsAsync(
            DateTime? startDate,
            DateTime? endDate,
            double? lat,
            double? lng,
            int? pageIndex,
            int? pageSize,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            return await _repository.GetCleanupEventsAsync(startDate, endDate, string.Empty, searchQuery, sort, descending, lat, lng, pageIndex, pageSize);
        }

        [HttpGet("my")]
        [AllowAnonymous]
        public async Task<IEnumerable<CleanupEvent>> GetMyCleanupEventsAsync(
            DateTime? startDate,
            DateTime? endDate,
            double? lat,
            double? lng,
            int? pageIndex,
            int? pageSize,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            return await _repository.GetCleanupEventsAsync(startDate, endDate, user.UserName, searchQuery, sort, descending, lat, lng, pageIndex, pageSize);
        }

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IEnumerable<CleanupEvent>> GetPopularCleanupEventsAsync()
        {
            return await _repository.GetPopularCleanupEventsAsync();
        }

        [HttpGet("count")]
        [AllowAnonymous]
        public int GetCleanupEventsCount()
        {
            return _repository.GetCleanupEventsCount();
        }

        [HttpGet("isUserHost")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserHost(int id)
        {
            if (Context.User.GetUserId() == null)
            {
                return new ObjectResult(false);
            }

            var dinner = await _repository.GetCleanupEventAsync(id);
            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            return new ObjectResult(dinner.IsUserHost(user.UserName));
        }

        [HttpGet("isUserRegistered")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserRegistered(int id)
        {
            if (Context.User.GetUserId() == null)
            {
                return new ObjectResult(false);
            }

            var dinner = await _repository.GetCleanupEventAsync(id);
            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            return new ObjectResult(dinner.IsUserRegistered(user.UserName));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCleanupEventAsync([FromBody] CleanupEvent dinner)
        {
            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            dinner.UserName = user.UserName;

            GeoLocation.SearchByPlaceNameOrZip(dinner);
            dinner = await _repository.CreateCleanupEventAsync(dinner);
            var url = Url.RouteUrl("GetCleanupEventById", new { id = dinner.CleanupEventId }, Request.Scheme, Request.Host.ToUriComponent());

            Context.Response.StatusCode = (int)HttpStatusCode.Created;
            Context.Response.Headers["Location"] = url;
            return new ObjectResult(dinner);
        }

        [HttpPut("{id:int}", Name = "UpdateCleanupEventById")]
        public async Task<IActionResult> UpdateCleanupEventAsync(int id, [FromBody] CleanupEvent dinner)
        {
            if (dinner.CleanupEventId != id)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            if (!dinner.IsUserHost(user.UserName))
            {
                return HttpNotFound();
            }

            GeoLocation.SearchByPlaceNameOrZip(dinner);
            dinner = await _repository.UpdateCleanupEventAsync(dinner);
            return new ObjectResult(dinner);
        }

        [HttpDelete("{id:int}", Name = "DeleteCleanupEventById")]
        public async Task<IActionResult> DeleteCleanupEventAsync(int id)
        {
            var dinner = await _repository.GetCleanupEventAsync(id);
            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());

            if (!dinner.IsUserHost(user.UserName))
            {
                return HttpNotFound();
            }

            await _repository.DeleteCleanupEventAsync(id);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}
