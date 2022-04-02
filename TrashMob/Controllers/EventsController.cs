
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
    using TrashMob.Shared.Engine;
    using TrashMob.Poco;
    using Microsoft.ApplicationInsights;

    [Route("api/events")]
    public class EventsController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmailManager emailManager;
        private readonly IMapRepository mapRepository;

        public EventsController(IEventRepository eventRepository,
                                IEventAttendeeRepository eventAttendeeRepository,
                                IUserRepository userRepository,
                                IEmailManager emailManager,
                                IMapRepository mapRepository,
                                TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventRepository = eventRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userRepository = userRepository;
            this.emailManager = emailManager;
            this.mapRepository = mapRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
        {
            var result = await eventRepository.GetAllEvents(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveEvents(CancellationToken cancellationToken)
        {
            var results = await eventRepository.GetActiveEvents(cancellationToken).ConfigureAwait(false);

            var displayResults = new List<DisplayEvent>();

            foreach (var mobEvent in results)
            {
                var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId, cancellationToken);
                displayResults.Add(mobEvent.ToDisplayEvent(user.UserName));
            }

            return Ok(displayResults);
        }

        [HttpGet]
        [Route("eventsuserisattending/{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByInternalId(userId, cancellationToken).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result = await eventAttendeeRepository.GetEventsUserIsAttending(userId, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userevents/{userId}/{futureEventsOnly}")]
        public async Task<IActionResult> GetUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByInternalId(userId, cancellationToken).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result1 = await eventRepository.GetUserEvents(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);
            var result2 = await eventAttendeeRepository.GetEventsUserIsAttending(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        [HttpGet]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("canceleduserevents/{userId}/{futureEventsOnly}")]
        public async Task<IActionResult> GetCanceledUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByInternalId(userId, cancellationToken).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result1 = await eventRepository.GetCanceledUserEvents(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);
            var result2 = await eventAttendeeRepository.GetCanceledEventsUserIsAttending(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken)
        {
            var mobEvent = await eventRepository.GetEvent(id, cancellationToken).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        [HttpPut]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEvent(Event mobEvent)
        {
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                var updatedEvent = await eventRepository.UpdateEvent(mobEvent).ConfigureAwait(false);
                TelemetryClient.TrackEvent(nameof(UpdateEvent));

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
        public async Task<IActionResult> AddEvent(Event mobEvent)
        {
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var eventId = await eventRepository.AddEvent(mobEvent).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEvent));

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

        [HttpDelete]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEvent(EventCancellationRequest eventCancellationRequest)
        {
            var mobEvent = await eventRepository.GetEvent(eventCancellationRequest.EventId).ConfigureAwait(false);
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var eventAttendees = await eventAttendeeRepository.GetEventAttendees(eventCancellationRequest.EventId).ConfigureAwait(false);
            
            await eventRepository.DeleteEvent(eventCancellationRequest.EventId, eventCancellationRequest.CancellationReason).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEvent));

            var message = emailManager.GetEmailTemplate(NotificationTypeEnum.EventCancelledNotice.ToString());
            message = message.Replace("{EventName}", mobEvent.Name);

            var localTime = await mapRepository.GetTimeForPoint(new Tuple<double, double>(mobEvent.Latitude.Value, mobEvent.Longitude.Value), mobEvent.EventDate).ConfigureAwait(false);
            DateTime localDate = (!string.IsNullOrWhiteSpace(localTime)) ? DateTime.Parse(localTime) : mobEvent.EventDate.DateTime;

            message = message.Replace("{EventDate}", localDate.ToString("MMMM dd, yyyy HH:mm tt"));
            message = message.Replace("{CancellationReason}", eventCancellationRequest.CancellationReason);

            var htmlMessage = emailManager.GetHtmlEmailTemplate(NotificationTypeEnum.EventCancelledNotice.ToString());
            htmlMessage = htmlMessage.Replace("{EventName}", mobEvent.Name);

            htmlMessage = htmlMessage.Replace("{EventDate}", localDate.ToString("MMMM dd, yyyy HH:mm tt"));
            htmlMessage = htmlMessage.Replace("{CancellationReason}", eventCancellationRequest.CancellationReason);

            var subject = "A TrashMob.eco event you were scheduled to attend has been cancelled!";

            foreach (var attendee in eventAttendees)
            {
                var userMessage = message.Replace("{UserName}", attendee.UserName);
                var userHtmlMessage = htmlMessage.Replace("{UserName}", attendee.UserName);

                var recipients = new List<EmailAddress>
                {
                    new EmailAddress { Name = attendee.UserName, Email = attendee.Email },
                };

                await emailManager.SendSystemEmail(subject, userMessage, userHtmlMessage, recipients, CancellationToken.None).ConfigureAwait(false);
            }

            return Ok(eventCancellationRequest.EventId);
        }

        private async Task<bool> EventExists(Guid id)
        {
            return (await eventRepository.GetAllEvents().ConfigureAwait(false)).Any(e => e.Id == id);
        }
    }
}
