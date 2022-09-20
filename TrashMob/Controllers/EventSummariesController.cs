
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventsummaries")]
    public class EventSummariesController : BaseController
    {
        private readonly IEventSummaryRepository eventSummaryRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventRepository eventRepository;

        public EventSummariesController(IEventSummaryRepository eventSummaryRepository,
                                       IUserRepository userRepository,
                                       IEventRepository eventRepository,
                                       TelemetryClient telemetryClient) 
            : base(telemetryClient)
        {
            this.eventSummaryRepository = eventSummaryRepository;
            this.userRepository = userRepository;
            this.eventRepository = eventRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventSummary(Guid eventId, CancellationToken cancellationToken = default)
        {
            var eventSummary = await eventSummaryRepository.GetEventSummary(eventId, cancellationToken).ConfigureAwait(false);

            if (eventSummary != null)
            {
                return Ok(eventSummary);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetEventSummaries([FromQuery] string country = "", [FromQuery] string region = "", [FromQuery] string city = "", [FromQuery] string postalCode = "", CancellationToken cancellationToken = default)
        {
            // Todo make this more efficient
            var eventSummaries = eventSummaryRepository.GetEventSummaries(cancellationToken).ToList();
            var displaySummaries = new List<DisplayEventSummary>();
            foreach (var eventSummary in eventSummaries)
            {
                var mobEvent = await eventRepository.GetEvent(eventSummary.EventId, cancellationToken).ConfigureAwait(false);

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
                            NumberOfBags = eventSummary.NumberOfBags + eventSummary.NumberOfBuckets / 3.0,
                            PostalCode = mobEvent.PostalCode,
                            Region = mobEvent.Region,
                            StreetAddress = mobEvent.StreetAddress,
                            TotalWorkHours = eventSummary.ActualNumberOfAttendees * eventSummary.DurationInMinutes / 60.0
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
        public async Task<IActionResult> UpdateEventSummary(EventSummary eventSummary)
        {
            var user = await userRepository.GetUserByInternalId(eventSummary.CreatedByUserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var updatedEvent = await eventSummaryRepository.UpdateEventSummary(eventSummary).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdateEventSummary));

            return Ok(updatedEvent);
        }

        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventSummary(EventSummary eventSummary)
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
            TelemetryClient.TrackEvent(nameof(AddEventSummary));

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
            TelemetryClient.TrackEvent(nameof(DeleteEventSummary));

            return Ok(eventId);
        }
    }
}
