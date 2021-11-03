
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
        private readonly IEventRepository eventRepository;

        public EventSummariesController(IEventSummaryRepository eventSummaryRepository,
                                       IUserRepository userRepository,
                                       IEventRepository eventRepository)
        {
            this.eventSummaryRepository = eventSummaryRepository;
            this.userRepository = userRepository;
            this.eventRepository = eventRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventSummary(Guid eventId)
        {
            var eventSummary = await eventSummaryRepository.GetEventSummary(eventId).ConfigureAwait(false);
            return Ok(eventSummary);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventSummaries([FromQuery] string country = "", [FromQuery] string region = "", [FromQuery] string city = "", [FromQuery] string postalCode = "")
        {
            // Todo make this more efficient
            var eventSummaries = eventSummaryRepository.GetEventSummaries().ToList();
            var displaySummaries = new List<DisplayEventSummary>();
            foreach (var eventSummary in eventSummaries)
            {
                var mobEvent = await eventRepository.GetEvent(eventSummary.EventId).ConfigureAwait(false);

                if (mobEvent != null)
                {
                    if ((string.IsNullOrWhiteSpace(country) || string.Equals(mobEvent.Country, country, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(region) || string.Equals(mobEvent.Region, region, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(city) || mobEvent.City.Contains(city, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(postalCode) || mobEvent.PostalCode.Contains(postalCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        var displayEvent = new DisplayEventSummary()
                        {
                            ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees,
                            City = mobEvent.City,
                            Country = mobEvent.Country,
                            DurationInMinutes = eventSummary.DurationInMinutes,
                            EventDate = mobEvent.EventDate,
                            EventId = mobEvent.Id,
                            EventTypeId = mobEvent.EventTypeId,
                            Name = mobEvent.Name,
                            NumberOfBags = eventSummary.NumberOfBags + eventSummary.NumberOfBuckets / 3,
                            PostalCode = mobEvent.PostalCode,
                            Region = mobEvent.Region,
                            StreetAddress = mobEvent.StreetAddress
                        };

                        displaySummaries.Add(displayEvent);
                    }
                }
            }

            return Ok(displaySummaries);
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
            var eventSummary = await eventSummaryRepository.GetEventSummary(eventId).ConfigureAwait(false);
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
