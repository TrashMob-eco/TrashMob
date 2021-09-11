
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Common;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
    using System.Collections.Generic;

    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmailManager emailManager;

        public EventsController(IEventRepository eventRepository, IEventAttendeeRepository eventAttendeeRepository, IUserRepository userRepository, IEmailManager emailManager)
        {
            this.eventRepository = eventRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userRepository = userRepository;
            this.emailManager = emailManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var result = await eventRepository.GetAllEvents().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveEvents()
        {
            var result = await eventRepository.GetActiveEvents().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("eventsuserisattending/{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result = await eventAttendeeRepository.GetEventsUserIsAttending(userId).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userevents/{userId}/{futureEventsOnly}")]
        public async Task<IActionResult> GetUserEvents(Guid userId, bool futureEventsOnly)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result1 = await eventRepository.GetUserEvents(userId, futureEventsOnly).ConfigureAwait(false);
            var result2 = await eventAttendeeRepository.GetEventsUserIsAttending(userId, futureEventsOnly).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var mobEvent = await eventRepository.GetEvent(id).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        [HttpPut]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEvent(Event mobEvent)
        {
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                var updatedEvent = await eventRepository.UpdateEvent(mobEvent).ConfigureAwait(false);
                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(mobEvent.Id).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostEvent(Event mobEvent)
        {
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var eventId = await eventRepository.AddEvent(mobEvent).ConfigureAwait(false);

            var message = $"A new event: {mobEvent.Name} in {mobEvent.City} has been created on TrashMob.eco!";
            var htmlMessage = $"A new event: {mobEvent.Name} in {mobEvent.City} has been created on TrashMob.eco!";
            var subject = "New Event Alert";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendGenericSystemEmail(subject, message, htmlMessage, recipients, CancellationToken.None).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetEvent), new { eventId });
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var mobEvent = await eventRepository.GetEvent(id).ConfigureAwait(false);
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventRepository.DeleteEvent(id).ConfigureAwait(false);
            return Ok(id);
        }

        private async Task<bool> EventExists(Guid id)
        {
            return (await eventRepository.GetAllEvents().ConfigureAwait(false)).Any(e => e.Id == id);
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
