namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages event attendee registrations and tracks which events users are attending.
    /// </summary>
    public class EventAttendeeManager(
        IBaseRepository<EventAttendee> repository,
        IKeyedRepository<Event> eventRepository,
        IEmailManager emailManager)
        : BaseManager<EventAttendee>(repository), IBaseManager<EventAttendee>, IEventAttendeeManager
    {

        /// <inheritdoc />
        public override async Task<IEnumerable<EventAttendee>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.User)
                    .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            var eventAttendee = await Repository.Get(ea => ea.EventId == parentId && ea.UserId == secondId)
                .FirstOrDefaultAsync(cancellationToken);

            return await Repository.DeleteAsync(eventAttendee);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(Guid attendeeId,
            bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            var eventAttendees = await Repository.Get(ea => ea.UserId == attendeeId).ToListAsync(cancellationToken);

            if (eventAttendees.Any())
            {
                var events = await eventRepository.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled
                                                            && (!futureEventsOnly ||
                                                                e.EventDate >= DateTimeOffset.UtcNow)
                                                            && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                    .ToListAsync(cancellationToken);
                return events;
            }

            return [];
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(EventFilter filter, Guid attendeeId,
            CancellationToken cancellationToken = default)
        {
            var eventAttendees = await Repository.Get(ea => ea.UserId == attendeeId).ToListAsync(cancellationToken);

            if (eventAttendees.Any())
            {
                var events = await eventRepository.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled &&
                                                           (filter.StartDate == null || e.EventDate >= filter.StartDate) &&
                                                           (filter.EndDate == null || e.EventDate <= filter.EndDate) &&
                                                            eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                    .ToListAsync(cancellationToken);
                return events;
            }

            return [];
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetCanceledEventsUserIsAttendingAsync(Guid attendeeId,
            bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            // TODO: Are there are better ways to do this?
            var eventAttendees = Repository.Get(ea => ea.UserId == attendeeId);

            var events = await eventRepository.Get(e => e.EventStatusId == (int)EventStatusEnum.Canceled
                                                        && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow)
                                                        && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                .ToListAsync(cancellationToken);
            return events;
        }

        /// <inheritdoc />
        public async Task<bool> IsEventLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
        {
            // Check if user is a lead through EventAttendees
            var attendee = await Repository.Get(ea => ea.EventId == eventId && ea.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendee != null && attendee.IsEventLead)
            {
                return true;
            }

            // Also check if user is the event creator (they're always considered a lead)
            var evt = await eventRepository.GetAsync(eventId, cancellationToken);
            return evt != null && evt.CreatedByUserId == userId;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventAttendee>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(ea => ea.EventId == eventId && ea.IsEventLead)
                .Include(ea => ea.User)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, Guid promotedByUserId, CancellationToken cancellationToken = default)
        {
            // Check max leads limit (5)
            var currentLeadCount = await Repository.Get(ea => ea.EventId == eventId && ea.IsEventLead)
                .CountAsync(cancellationToken);

            if (currentLeadCount >= 5)
            {
                throw new InvalidOperationException("Maximum of 5 co-leads per event has been reached.");
            }

            var attendee = await Repository.Get(ea => ea.EventId == eventId && ea.UserId == userId)
                .Include(ea => ea.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendee == null)
            {
                throw new InvalidOperationException("User is not an attendee of this event.");
            }

            if (attendee.IsEventLead)
            {
                throw new InvalidOperationException("User is already an event lead.");
            }

            attendee.IsEventLead = true;
            attendee.LastUpdatedByUserId = promotedByUserId;
            attendee.LastUpdatedDate = DateTimeOffset.UtcNow;

            await Repository.UpdateAsync(attendee);

            // Send notification email to the promoted user
            await SendCoLeadNotificationAsync(eventId, attendee.User, NotificationTypeEnum.EventCoLeadAdded, cancellationToken);

            return attendee;
        }

        /// <inheritdoc />
        public async Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, Guid demotedByUserId, CancellationToken cancellationToken = default)
        {
            // Check that we're not removing the last lead
            var currentLeadCount = await Repository.Get(ea => ea.EventId == eventId && ea.IsEventLead)
                .CountAsync(cancellationToken);

            if (currentLeadCount <= 1)
            {
                throw new InvalidOperationException("Cannot remove the last event lead. At least one lead is required.");
            }

            var attendee = await Repository.Get(ea => ea.EventId == eventId && ea.UserId == userId)
                .Include(ea => ea.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendee == null)
            {
                throw new InvalidOperationException("User is not an attendee of this event.");
            }

            if (!attendee.IsEventLead)
            {
                throw new InvalidOperationException("User is not an event lead.");
            }

            attendee.IsEventLead = false;
            attendee.LastUpdatedByUserId = demotedByUserId;
            attendee.LastUpdatedDate = DateTimeOffset.UtcNow;

            await Repository.UpdateAsync(attendee);

            // Send notification email to the demoted user
            await SendCoLeadNotificationAsync(eventId, attendee.User, NotificationTypeEnum.EventCoLeadRemoved, cancellationToken);

            return attendee;
        }

        private async Task SendCoLeadNotificationAsync(Guid eventId, User user, NotificationTypeEnum notificationType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(user?.Email))
            {
                return;
            }

            var evt = await eventRepository.GetAsync(eventId, cancellationToken);
            if (evt == null)
            {
                return;
            }

            var eventDate = evt.EventDate.ToLocalTime();
            var subject = notificationType == NotificationTypeEnum.EventCoLeadAdded
                ? $"You've been made a co-lead for {evt.Name}"
                : $"You've been removed as co-lead for {evt.Name}";

            var message = emailManager.GetHtmlEmailCopy(notificationType.ToString());
            message = message.Replace("{EventName}", evt.Name);
            message = message.Replace("{EventDate}", eventDate.ToString("D"));
            message = message.Replace("{EventTime}", eventDate.ToString("t"));

            List<EmailAddress> recipients =
            [
                new() { Name = user.UserName ?? "TrashMob User", Email = user.Email },
            ];

            var dynamicTemplateData = new
            {
                username = user.UserName ?? "TrashMob User",
                emailCopy = message,
                subject
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);
        }
    }
}