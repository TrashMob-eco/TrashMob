
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/eventsummaries")]
    public class EventSummariesController : ControllerBase
    {
        private readonly IEventSummaryRepository eventSummaryRepository;
        private readonly IUserRepository userRepository;

        public EventSummariesController(IEventSummaryRepository eventSummaryRepository,
                                       IUserRepository userRepository)
        {
            this.eventSummaryRepository = eventSummaryRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventSummary(Guid eventId)
        {
            var eventSummary = await eventSummaryRepository.GetEventSummary(eventId).ConfigureAwait(false);
            return Ok(eventSummary);
        }

        [HttpPut]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEventSummary(EventSummary eventSummary)
        {
            var user = await userRepository.GetUserByInternalId(eventSummary.CreatedByUserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var updatedEvent = await eventSummaryRepository.UpdateEventSummary(eventSummary).ConfigureAwait(false);
            return Ok(updatedEvent);
        }

        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostEventSummary(EventSummary eventSummary)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
            if (currentUser == null || !ValidateUser(currentUser.NameIdentifier))
            {
                return Forbid();
            }

            eventSummary.CreatedByUserId = currentUser.Id;
            eventSummary.LastUpdatedByUserId = currentUser.Id;
            eventSummary.CreatedDate = DateTimeOffset.UtcNow;
            eventSummary.LastUpdatedDate = DateTimeOffset.UtcNow;
            await eventSummaryRepository.AddEventSummary(eventSummary).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetEventSummary), new { eventId = eventSummary.EventId });
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventSummary(Guid eventId)
        {
            var eventSummary = await eventSummaryRepository.GetEventSummary(eventId);
            var user = await userRepository.GetUserByInternalId(eventSummary.CreatedByUserId).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventSummaryRepository.DeleteEventSummary(eventId).ConfigureAwait(false);
            return Ok(eventId);
        }

        // Ensure the user calling in is the owner of the record
        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
