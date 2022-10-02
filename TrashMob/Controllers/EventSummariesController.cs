
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventsummaries")]
    public class EventSummariesController : SecureController
    {
        private readonly IEventSummaryRepository eventSummaryRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventRepository eventRepository;

        public EventSummariesController(TelemetryClient telemetryClient,
                                        IUserRepository userRepository,
                                        IAuthorizationService authorizationService,
                                        IEventSummaryRepository eventSummaryRepository,
                                        IEventRepository eventRepository) 
            : base(telemetryClient, authorizationService)
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
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventSummary(EventSummary eventSummary)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventSummary, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedEvent = await eventSummaryRepository.UpdateEventSummary(eventSummary).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdateEventSummary));

            return Ok(updatedEvent);
        }

        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventSummary(EventSummary eventSummary)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventSummary, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            eventSummary.CreatedByUserId = UserId;
            eventSummary.LastUpdatedByUserId = UserId;
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

            var authResult = await AuthorizationService.AuthorizeAsync(User, eventSummary, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await eventSummaryRepository.DeleteEventSummary(eventId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventSummary));

            return Ok(eventId);
        }
    }
}
