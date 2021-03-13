namespace FlashTrashMob.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
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
            var cleanupEvent = await _repository.GetCleanupEventAsync(id);
            if (cleanupEvent == null)
            {
                return NotFound();
            }

            return new ObjectResult(cleanupEvent);
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
            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value == null)
            {
                return new ObjectResult(false);
            }

            var cleanupEvent = await _repository.GetCleanupEventAsync(id);
            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return new ObjectResult(cleanupEvent.IsUserHost(user.UserName));
        }

        [HttpGet("isUserRegistered")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserRegistered(int id)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value == null)
            {
                return new ObjectResult(false);
            }

            var cleanupEvent = await _repository.GetCleanupEventAsync(id);
            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return new ObjectResult(cleanupEvent.IsUserRegistered(user.UserName));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCleanupEventAsync([FromBody] CleanupEvent cleanupEvent)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            cleanupEvent.UserName = user.UserName;

            GeoLocation.SearchByPlaceNameOrZip(cleanupEvent);
            cleanupEvent = await _repository.CreateCleanupEventAsync(cleanupEvent);
            var url = Url.RouteUrl("GetCleanupEventById", new { id = cleanupEvent.CleanupEventId }, Request.Scheme, Request.Host.ToUriComponent());

            Response.StatusCode = (int)HttpStatusCode.Created;
            Response.Headers["Location"] = url;
            return new ObjectResult(cleanupEvent);
        }

        [HttpPut("{id:int}", Name = "UpdateCleanupEventById")]
        public async Task<IActionResult> UpdateCleanupEventAsync(int id, [FromBody] CleanupEvent cleanupEvent)
        {
            if (cleanupEvent.CleanupEventId != id)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (!cleanupEvent.IsUserHost(user.UserName))
            {
                return NotFound();
            }

            GeoLocation.SearchByPlaceNameOrZip(cleanupEvent);
            cleanupEvent = await _repository.UpdateCleanupEventAsync(cleanupEvent);
            return new ObjectResult(cleanupEvent);
        }

        [HttpDelete("{id:int}", Name = "DeleteCleanupEventById")]
        public async Task<IActionResult> DeleteCleanupEventAsync(int id)
        {
            var cleanupEvent = await _repository.GetCleanupEventAsync(id);
            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!cleanupEvent.IsUserHost(user.UserName))
            {
                return NotFound();
            }

            await _repository.DeleteCleanupEventAsync(id);
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
