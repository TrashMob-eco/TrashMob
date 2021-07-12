
namespace TrashMob.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
    using System.Collections.Generic;

    [ApiController]
    [Route("api/eventmedias")]
    public class EventMediasController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IEventMediaRepository eventMediaRepository;

        public EventMediasController(IUserRepository userRepository, IEventMediaRepository eventMediaRepository)
        {
            this.userRepository = userRepository;
            this.eventMediaRepository = eventMediaRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventMedias(Guid eventId)
        {
            var result = await eventMediaRepository.GetEventMediasByEvent(eventId).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetEventMedias()
        {
            var result = await eventMediaRepository.GetEventMedias().ConfigureAwait(false);
            return Ok(result);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEventMedia(Guid userId, IList<EventMedia> eventMedias)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            foreach (var eventMedia in eventMedias)
            {
                await eventMediaRepository.AddUpdateEventMedia(eventMedia).ConfigureAwait(false);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventMedia(Guid id)
        {
            var eventMedia = await eventMediaRepository.GetEventMediaById(id);
            var user = await userRepository.GetUserByInternalId(eventMedia.CreatedByUserId).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventMediaRepository.DeleteEventMedia(id).ConfigureAwait(false);
            return Ok(id);
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
