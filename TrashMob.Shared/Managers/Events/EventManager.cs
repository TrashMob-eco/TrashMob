namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    public class EventManager(
        IKeyedRepository<Event> repository,
        IEventAttendeeManager eventAttendeeManager,
        IBaseRepository<EventAttendee> eventAttendeeRepository,
        IEventLitterReportManager eventLitterReportManager,
        IMapManager mapManager,
        IEmailManager emailManager)
        : KeyedManager<Event>(repository), IEventManager
    {
        private const int StandardEventWindowInMinutes = 120;
        private readonly IEventLitterReportManager eventLitterReportManager = eventLitterReportManager;

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e =>
                    (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full)
                    && e.IsEventPublic
                    && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes))
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCompletedEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.EventDate < DateTimeOffset.UtcNow
                                       && e.EventStatusId != (int)EventStatusEnum.Canceled)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                       && e.EventStatusId != (int)EventStatusEnum.Canceled
                                       && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCanceledUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                       && e.EventStatusId == (int)EventStatusEnum.Canceled
                                       && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetFilteredEventsAsync(EventFilter filter,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => (filter.StartDate == null || e.EventDate >= filter.StartDate) &&
                                       (filter.EndDate == null || e.EventDate <= filter.EndDate) &&
                                       (filter.Country == null || e.Country == filter.Country) &&
                                       (filter.Region == null || e.Region == filter.Region) &&
                                       (filter.City == null || e.City == filter.City) &&
                                       (filter.CreatedByUserId == null || e.CreatedByUserId == filter.CreatedByUserId))
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Location>> GeEventLocationsByTimeRangeAsync(DateTimeOffset? startTime,
            DateTimeOffset? endTime, CancellationToken cancellationToken = default)
        {
            var locations = await Repo.Get()
                .Where(e => (startTime == null || e.CreatedDate >= startTime) &&
                            (endTime == null || e.CreatedDate <= endTime))
                .GroupBy(e => new { e.Country, e.Region, e.City })
                .Select(group => new Location
                    { Country = group.Key.Country, Region = group.Key.Region, City = group.Key.City })
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return locations;
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteAsync(Guid id, string cancellationReason, Guid userId,
            CancellationToken cancellationToken)
        {
            var instance = await Repo.GetAsync(id, cancellationToken).ConfigureAwait(false);

            instance.EventStatusId = (int)EventStatusEnum.Canceled;
            instance.CancellationReason = cancellationReason;

            await base.UpdateAsync(instance, userId, cancellationToken);

            var eventLitterReports = await eventLitterReportManager.GetByParentIdAsync(id, cancellationToken).ConfigureAwait(false);

            foreach (var eventLitterReport in eventLitterReports)
            {
                await eventLitterReportManager.Delete(id, eventLitterReport.LitterReportId, cancellationToken).ConfigureAwait(false);
            }

            var eventAttendees = eventAttendeeRepository.Get(e => e.EventId == id).Include(e => e.User);

            var subject = "A TrashMob.eco event you were scheduled to attend has been cancelled!";

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventCancelledNotice.ToString());
            emailCopy = emailCopy.Replace("{CancellationReason}", cancellationReason);

            var localDate = await instance.GetLocalEventTime(mapManager).ConfigureAwait(false);

            foreach (var attendee in eventAttendees)
            {
                var dynamicTemplateData = new
                {
                    username = attendee.User.UserName,
                    eventName = instance.Name,
                    eventDate = localDate.Item1,
                    eventTime = localDate.Item2,
                    eventAddress = instance.EventAddress(),
                    emailCopy,
                    subject,
                    eventDetailsUrl = instance.EventDetailsUrl(),
                    googleMapsUrl = instance.GoogleMapsUrl(),
                };

                var recipients = new List<EmailAddress>
                {
                    new() { Name = attendee.User.UserName, Email = attendee.User.Email },
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail,
                        SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None)
                    .ConfigureAwait(false);
            }

            return 1;
        }

        public override async Task<Event> AddAsync(Event instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var newEvent = await base.AddAsync(instance, userId, cancellationToken);

            var newEventAttendee = new EventAttendee
            {
                UserId = userId,
                EventId = instance.Id,
                SignUpDate = DateTime.UtcNow,
            };

            await eventAttendeeManager.AddAsync(newEventAttendee, userId, cancellationToken);

            var message = $"A new event: {instance.Name} in {instance.City} has been created on TrashMob.eco!";
            var subject = "New Event Alert";

            var recipients = new List<EmailAddress>
            {
                new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
            };

            var localTime = await instance.GetLocalEventTime(mapManager).ConfigureAwait(false);

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                eventName = instance.Name,
                eventDate = localTime.Item1,
                eventTime = localTime.Item2,
                eventAddress = instance.EventAddress(),
                emailCopy = message,
                subject,
                eventDetailsUrl = instance.EventDetailsUrl(),
                googleMapsUrl = instance.GoogleMapsUrl(),
            };

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail,
                    SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None)
                .ConfigureAwait(false);

            return newEvent;
        }

        public override async Task<Event> UpdateAsync(Event instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var oldEvent = await Repo.GetWithNoTrackingAsync(instance.Id, cancellationToken).ConfigureAwait(false);

            var updatedEvent = await base.UpdateAsync(instance, userId, cancellationToken);

            if (oldEvent.EventDate != instance.EventDate
                || oldEvent.City != instance.City
                || oldEvent.Country != instance.Country
                || oldEvent.Region != instance.Region
                || oldEvent.PostalCode != instance.PostalCode
                || oldEvent.StreetAddress != instance.StreetAddress)
            {
                var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventUpdatedNotice.ToString());
                emailCopy = emailCopy.Replace("{EventName}", instance.Name);

                var oldLocalDate = await oldEvent.GetLocalEventTime(mapManager).ConfigureAwait(false);
                var newLocalDate = await instance.GetLocalEventTime(mapManager).ConfigureAwait(false);

                emailCopy = emailCopy.Replace("{EventDate}", oldLocalDate.Item1);
                emailCopy = emailCopy.Replace("{EventTime}", oldLocalDate.Item2);

                var subject = "A TrashMob.eco event you were scheduled to attend has been updated!";

                var eventAttendees = eventAttendeeRepository.Get(m => m.EventId == instance.Id).Include(a => a.User);

                foreach (var attendee in eventAttendees)
                {
                    var dynamicTemplateData = new
                    {
                        username = attendee.User.UserName,
                        eventName = instance.Name,
                        eventDate = newLocalDate.Item1,
                        eventTime = newLocalDate.Item2,
                        eventAddress = instance.EventAddress(),
                        emailCopy,
                        subject,
                        eventDetailsUrl = instance.EventDetailsUrl(),
                        googleMapsUrl = instance.GoogleMapsUrl(),
                    };

                    var recipients = new List<EmailAddress>
                    {
                        new() { Name = attendee.User.UserName, Email = attendee.User.Email },
                    };

                    await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail,
                            SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }

            return updatedEvent;
        }
    }
}