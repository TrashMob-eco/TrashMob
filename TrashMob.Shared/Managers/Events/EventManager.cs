namespace TrashMob.Shared.Managers.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Models.Extensions;
    using TrashMob.Shared.Poco;

    public class EventManager : KeyedManager<Event>, IKeyedManager<Event>, IEventManager
    {
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IBaseRepository<EventAttendee> eventAttendeeRepository;
        private readonly IMapManager mapManager;
        private readonly IEmailManager emailManager;
        private const int StandardEventWindowInMinutes = 120;

        public EventManager(IKeyedRepository<Event> repository,
                            IEventAttendeeManager eventAttendeeManager,
                            IBaseRepository<EventAttendee> eventAttendeeRepository,
                            IMapManager mapManager,
                            IEmailManager emailManager) : base(repository)
        {
            this.eventAttendeeManager = eventAttendeeManager;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.mapManager = mapManager;
            this.emailManager = emailManager;
        }

        public override async Task<Event> AddAsync(Event instance, Guid userId, CancellationToken cancellationToken = default)
        {
            var newEvent = await base.AddAsync(instance, userId, cancellationToken);

            var newEventAttendee = new EventAttendee
            {
                UserId = userId,
                EventId = instance.Id
            };

            await eventAttendeeManager.AddAsync(newEventAttendee, userId, cancellationToken);

            var message = $"A new event: {instance.Name} in {instance.City} has been created on TrashMob.eco!";
            var subject = "New Event Alert";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
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
                subject = subject,
                eventDetailsUrl = instance.EventDetailsUrl(),
                googleMapsUrl = instance.GoogleMapsUrl(),
            };

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            return newEvent;
        }
        public override async Task<Event> UpdateAsync(Event instance, Guid userId, CancellationToken cancellationToken = default)
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
                        emailCopy = emailCopy,
                        subject = subject,
                        eventDetailsUrl = instance.EventDetailsUrl(),
                        googleMapsUrl = instance.GoogleMapsUrl(),
                    };

                    var recipients = new List<EmailAddress>
                        {
                            new EmailAddress { Name = attendee.User.UserName, Email = attendee.User.Email },
                        };

                    await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);
                }
            }

            return updatedEvent;
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full) 
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

        public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                  && e.EventStatusId != (int)EventStatusEnum.Canceled
                                  && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                             .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCanceledUserEventsAsync(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                  && e.EventStatusId == (int)EventStatusEnum.Canceled
                                  && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                             .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteAsync(Guid id, string cancellationReason, Guid userId, CancellationToken cancellationToken)
        {
            var instance = await Repo.GetAsync(id, cancellationToken).ConfigureAwait(false);

            instance.EventStatusId = (int)EventStatusEnum.Canceled;
            instance.CancellationReason = cancellationReason;
            
            await base.UpdateAsync(instance, userId, cancellationToken);

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
                    emailCopy = emailCopy,
                    subject = subject,
                    eventDetailsUrl = instance.EventDetailsUrl(),
                    googleMapsUrl = instance.GoogleMapsUrl(),
                };

                var recipients = new List<EmailAddress>
                {
                    new EmailAddress { Name = attendee.User.UserName, Email = attendee.User.Email },
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);
            }

            return 1;
        }
    }
}
