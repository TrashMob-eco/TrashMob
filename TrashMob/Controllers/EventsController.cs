
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Common;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared;
    using System.Collections.Generic;
    using TrashMob.Shared.Engine;
    using TrashMob.Poco;
    using Microsoft.ApplicationInsights;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Shared.Poco;

    [Route("api/events")]
    public class EventsController : SecureController
    {
        private readonly IEventManager eventManager;
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IKeyedManager<User> userManager;
        private readonly IEmailManager emailManager;
        private readonly IMapManager mapRepository;

        public EventsController(IKeyedManager<User> userManager,
                                IEventManager eventManager,
                                IEventAttendeeManager eventAttendeeManager,                                
                                IEmailManager emailManager,
                                IMapManager mapRepository)
            : base()
        {
            this.eventManager = eventManager;
            this.eventAttendeeManager = eventAttendeeManager;
            this.userManager = userManager;
            this.emailManager = emailManager;
            this.mapRepository = mapRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
        {
            var result = await eventManager.Get(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveEvents(CancellationToken cancellationToken)
        {
            var results = await eventManager.GetActiveEvents(cancellationToken).ConfigureAwait(false);

            var displayResults = new List<DisplayEvent>();

            foreach (var mobEvent in results)
            {
                var user = await userManager.Get(mobEvent.CreatedByUserId, cancellationToken);
                displayResults.Add(mobEvent.ToDisplayEvent(user.UserName));
            }

            return Ok(displayResults);
        }

        [HttpGet]
        [Route("eventsuserisattending/{userId}")]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken)
        {
            var result = await eventAttendeeManager.GetEventsUserIsAttending(userId, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userevents/{userId}/{futureEventsOnly}")]
        public async Task<IActionResult> GetUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken)
        {
            var result1 = await eventManager.GetUserEvents(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);
            var result2 = await eventAttendeeManager.GetEventsUserIsAttending(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        [HttpGet]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("canceleduserevents/{userId}/{futureEventsOnly}")]
        public async Task<IActionResult> GetCanceledUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken)
        {
            var result1 = await eventManager.GetCanceledUserEvents(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);
            var result2 = await eventAttendeeManager.GetCanceledEventsUserIsAttending(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.Get(id, cancellationToken).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, "UserOwnsEntity");
            
            if (!User.Identity.IsAuthenticated || !authResult.Succeeded )
            {
                return Forbid();
            }

            try
            {
                var oldEvent = await eventManager.Get(mobEvent.Id, cancellationToken).ConfigureAwait(false);

                var updatedEvent = await eventManager.Update(mobEvent).ConfigureAwait(false);
                TelemetryClient.TrackEvent(nameof(UpdateEvent));

                if (oldEvent.EventDate != mobEvent.EventDate
                    || oldEvent.City != mobEvent.City
                    || oldEvent.Country != mobEvent.Country
                    || oldEvent.Region != mobEvent.Region
                    || oldEvent.PostalCode != mobEvent.PostalCode
                    || oldEvent.StreetAddress != mobEvent.StreetAddress)
                {
                    var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventUpdatedNotice.ToString());
                    emailCopy = emailCopy.Replace("{EventName}", mobEvent.Name);

                    var oldLocalDate = await oldEvent.GetLocalEventTime(mapRepository).ConfigureAwait(false);
                    var newLocalDate = await mobEvent.GetLocalEventTime(mapRepository).ConfigureAwait(false);

                    emailCopy = emailCopy.Replace("{EventDate}", oldLocalDate.Item1);
                    emailCopy = emailCopy.Replace("{EventTime}", oldLocalDate.Item2);

                    var subject = "A TrashMob.eco event you were scheduled to attend has been updated!";

                    var eventAttendees = await eventAttendeeManager.Get(m => m.EventId == mobEvent.Id, cancellationToken).ConfigureAwait(false);

                    foreach (var attendee in eventAttendees)
                    {
                        var dynamicTemplateData = new
                        {
                            username = attendee.User.UserName,
                            eventName = mobEvent.Name,
                            eventDate = newLocalDate.Item1,
                            eventTime = newLocalDate.Item2,
                            eventAddress = mobEvent.EventAddress(),
                            emailCopy = emailCopy,
                            subject = subject,
                            eventDetailsUrl = mobEvent.EventDetailsUrl(),
                            googleMapsUrl = mobEvent.GoogleMapsUrl(),
                        };

                        var recipients = new List<EmailAddress>
                        {
                            new EmailAddress { Name = attendee.User.UserName, Email = attendee.User.Email },
                        };

                        await emailManager.SendTemplatedEmail(subject, SendGridEmailTemplateId.EventEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);
                    }
                }

                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(mobEvent.Id, cancellationToken).ConfigureAwait(false))
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
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEvent(Event mobEvent)
        {
            var newEvent = await eventManager.Add(mobEvent).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEvent));

            var message = $"A new event: {mobEvent.Name} in {mobEvent.City} has been created on TrashMob.eco!";
            var subject = "New Event Alert";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            var localTime = await mobEvent.GetLocalEventTime(mapRepository).ConfigureAwait(false);

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                eventName = mobEvent.Name,
                eventDate = localTime.Item1,
                eventTime = localTime.Item2,
                eventAddress = mobEvent.EventAddress(),
                emailCopy = message,
                subject = subject,
                eventDetailsUrl = mobEvent.EventDetailsUrl(),
                googleMapsUrl = mobEvent.GoogleMapsUrl(),
            };

            await emailManager.SendTemplatedEmail(subject, SendGridEmailTemplateId.EventEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            return Ok(newEvent);
        }

        [HttpDelete]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEvent(EventCancellationRequest eventCancellationRequest, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.Get(eventCancellationRequest.EventId, cancellationToken).ConfigureAwait(false);

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var eventAttendees = await eventAttendeeManager.Get(e => e.EventId == eventCancellationRequest.EventId, cancellationToken).ConfigureAwait(false);

            await eventManager.Delete(eventCancellationRequest.EventId, eventCancellationRequest.CancellationReason, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEvent));

            var subject = "A TrashMob.eco event you were scheduled to attend has been cancelled!";

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventCancelledNotice.ToString());
            emailCopy = emailCopy.Replace("{CancellationReason}", eventCancellationRequest.CancellationReason);

            var localDate = await mobEvent.GetLocalEventTime(mapRepository).ConfigureAwait(false);

            foreach (var attendee in eventAttendees)
            {
                var dynamicTemplateData = new
                {
                    username = attendee.User.UserName,
                    eventName = mobEvent.Name,
                    eventDate = localDate.Item1,
                    eventTime = localDate.Item2,
                    eventAddress = mobEvent.EventAddress(),
                    emailCopy = emailCopy,
                    subject = subject,
                    eventDetailsUrl = mobEvent.EventDetailsUrl(),
                    googleMapsUrl = mobEvent.GoogleMapsUrl(),
                };

                var recipients = new List<EmailAddress>
                {
                    new EmailAddress { Name = attendee.User.UserName, Email = attendee.User.Email },
                };

                await emailManager.SendTemplatedEmail(subject, SendGridEmailTemplateId.EventEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);
            }

            return Ok(eventCancellationRequest.EventId);
        }

        private async Task<bool> EventExists(Guid id, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.Get(id, cancellationToken).ConfigureAwait(false);

            return mobEvent != null;
        }
    }
}
