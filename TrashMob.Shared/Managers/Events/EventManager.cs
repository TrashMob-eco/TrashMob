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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages event lifecycle including creation, updates, cancellation, and attendee notifications.
    /// </summary>
    /// <param name="repository">The repository for event data access.</param>
    /// <param name="eventAttendeeManager">The manager for event attendees.</param>
    /// <param name="eventAttendeeRepository">The repository for event attendee data access.</param>
    /// <param name="eventLitterReportManager">The manager for event litter reports.</param>
    /// <param name="mapManager">The map manager for timezone operations.</param>
    /// <param name="emailManager">The email manager for sending notifications.</param>
    /// <param name="teamManager">The manager for team operations.</param>
    public class EventManager(
        IKeyedRepository<Event> repository,
        IEventAttendeeManager eventAttendeeManager,
        IBaseRepository<EventAttendee> eventAttendeeRepository,
        IEventLitterReportManager eventLitterReportManager,
        IMapManager mapManager,
        IEmailManager emailManager,
        ITeamManager teamManager)
        : KeyedManager<Event>(repository), IEventManager
    {
        private const int StandardEventWindowInMinutes = 120;

        // Seed EventType row for a general "Cleanup" event. Used as the default type for
        // Instant Events per Project 65 Decisions (reuse rather than adding a "Solo Pick" type).
        // Corresponds to the seeded EventType with Name = "Cleanup".
        private const int DefaultEventTypeIdCleanup = 1;
        private readonly IEventLitterReportManager eventLitterReportManager = eventLitterReportManager;

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetActiveEventsAsync(Guid? userId = null,
            CancellationToken cancellationToken = default)
        {
            var userTeamIds = await GetUserTeamIdsAsync(userId, cancellationToken);

            return await Repo.Get(e =>
                    (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full)
                    && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes)
                    && (
                        e.EventVisibilityId == (int)EventVisibilityEnum.Public
                        || (e.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly
                            && e.TeamId != null
                            && userTeamIds.Contains(e.TeamId.Value))
                        || (userId != null && e.CreatedByUserId == userId.Value)
                    ))
                .Include(e => e.CreatedByUser)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetActiveTeamEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e =>
                    (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full)
                    && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes)
                    && e.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly
                    && e.TeamId != null)
                .Include(e => e.CreatedByUser)
                .Include(e => e.Team)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetCompletedEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.EventDate < DateTimeOffset.UtcNow
                                       && e.EventStatusId != (int)EventStatusEnum.Canceled)
                .Include(e => e.CreatedByUser)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                       && e.EventStatusId != (int)EventStatusEnum.Canceled
                                       && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetUserEventsAsync(EventFilter filter, Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                       && e.EventStatusId != (int)EventStatusEnum.Canceled
                                       && (filter.StartDate == null || filter.StartDate <= e.EventDate)
                                       && (filter.EndDate == null || filter.EndDate >= e.EventDate))
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetCanceledUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                       && e.EventStatusId == (int)EventStatusEnum.Canceled
                                       && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetFilteredEventsAsync(EventFilter filter, Guid? userId = null,
            CancellationToken cancellationToken = default)
        {
            var userTeamIds = await GetUserTeamIdsAsync(userId, cancellationToken);

            return await Repo.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled &&
                                       (filter.StartDate == null || e.EventDate >= filter.StartDate) &&
                                       (filter.EndDate == null || e.EventDate <= filter.EndDate) &&
                                       (filter.Country == null || e.Country == filter.Country) &&
                                       (filter.Region == null || e.Region == filter.Region) &&
                                       (filter.City == null || e.City == filter.City) &&
                                       (filter.CreatedByUserId == null || e.CreatedByUserId == filter.CreatedByUserId) &&
                                       (filter.EventStatusId == null || e.EventStatusId == filter.EventStatusId) &&
                                       (filter.EventVisibilityId == null || e.EventVisibilityId == filter.EventVisibilityId) &&
                                       (
                                           e.EventVisibilityId == (int)EventVisibilityEnum.Public
                                           || (e.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly
                                               && e.TeamId != null
                                               && userTeamIds.Contains(e.TeamId.Value))
                                           || (userId != null && e.CreatedByUserId == userId.Value)
                                       ))
                .Include(e => e.CreatedByUser)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Location>> GetEventLocationsByTimeRangeAsync(DateTimeOffset? startTime,
            DateTimeOffset? endTime, CancellationToken cancellationToken = default)
        {
            var locations = await Repo.Get()
                .Where(e => (startTime == null || e.CreatedDate >= startTime) &&
                            (endTime == null || e.CreatedDate <= endTime))
                .GroupBy(e => new { e.Country, e.Region, e.City })
                .Select(group => new Location
                    { Country = group.Key.Country, Region = group.Key.Region, City = group.Key.City })
                .ToListAsync(cancellationToken);

            return locations;
        }

        /// <inheritdoc />
        public async Task<IQueryable<Event>> GetFilteredEventsQueryableAsync(EventQueryParameters filter,
            Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var userTeamIds = await GetUserTeamIdsAsync(userId, cancellationToken);

            var query = Repo.Get(e =>
                e.EventStatusId != (int)EventStatusEnum.Canceled &&
                (filter.EventStatusId == null || e.EventStatusId == filter.EventStatusId) &&
                (filter.EventTypeId == null || e.EventTypeId == filter.EventTypeId) &&
                (filter.FromDate == null || e.EventDate >= filter.FromDate) &&
                (filter.ToDate == null || e.EventDate <= filter.ToDate) &&
                (filter.Country == null || e.Country == filter.Country) &&
                (filter.Region == null || e.Region == filter.Region) &&
                (filter.City == null || e.City == filter.City) &&
                (
                    e.EventVisibilityId == (int)EventVisibilityEnum.Public
                    || (e.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly
                        && e.TeamId != null
                        && userTeamIds.Contains(e.TeamId.Value))
                    || (userId != null && e.CreatedByUserId == userId.Value)
                ));

            return query.OrderByDescending(e => e.EventDate);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(Guid id, string cancellationReason, Guid userId,
            CancellationToken cancellationToken)
        {
            var instance = await Repo.GetAsync(id, cancellationToken);

            instance.EventStatusId = (int)EventStatusEnum.Canceled;
            instance.CancellationReason = cancellationReason;

            await base.UpdateAsync(instance, userId, cancellationToken);

            var eventLitterReports = await eventLitterReportManager.GetByParentIdAsync(id, cancellationToken);

            foreach (var eventLitterReport in eventLitterReports)
            {
                await eventLitterReportManager.Delete(id, eventLitterReport.LitterReportId, cancellationToken);
            }

            var eventAttendees = eventAttendeeRepository.Get(e => e.EventId == id).Include(e => e.User);

            var subject = "A TrashMob.eco event you were scheduled to attend has been cancelled!";

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventCancelledNotice.ToString());
            emailCopy = emailCopy.Replace("{CancellationReason}", cancellationReason);

            var localDate = await instance.GetLocalEventTime(mapManager);

            foreach (var attendee in eventAttendees)
            {
                var dynamicTemplateData = new
                {
                    username = attendee.User.UserName,
                    eventName = instance.Name,
                    eventDate = localDate.Date,
                    eventTime = localDate.Time,
                    eventAddress = instance.EventAddress(),
                    emailCopy,
                    subject,
                    eventDetailsUrl = instance.EventDetailsUrl(),
                    googleMapsUrl = instance.GoogleMapsUrl(),
                };

                List<EmailAddress> recipients =
                [
                    new() { Name = attendee.User.UserName, Email = attendee.User.Email },
                ];

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail,
                        SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None);
            }

            return 1;
        }

        /// <inheritdoc />
        public override async Task<Event> AddAsync(Event instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            await ValidateEventVisibilityAsync(instance, userId, cancellationToken);

            var newEvent = await base.AddAsync(instance, userId, cancellationToken);

            var newEventAttendee = new EventAttendee
            {
                UserId = userId,
                EventId = instance.Id,
                SignUpDate = DateTime.UtcNow,
                IsEventLead = true,
            };

            await eventAttendeeManager.AddAsync(newEventAttendee, userId, cancellationToken);

            var message = $"A new event: {instance.Name} in {instance.City} has been created on TrashMob.eco!";
            var subject = "New Event Alert";

            List<EmailAddress> recipients =
            [
                new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
            ];

            var localTime = await instance.GetLocalEventTime(mapManager);

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                eventName = instance.Name,
                eventDate = localTime.Date,
                eventTime = localTime.Time,
                eventAddress = instance.EventAddress(),
                emailCopy = message,
                subject,
                eventDetailsUrl = instance.EventDetailsUrl(),
                googleMapsUrl = instance.GoogleMapsUrl(),
            };

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail,
                    SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None);

            return newEvent;
        }

        /// <inheritdoc />
        public override async Task<Event> UpdateAsync(Event instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            await ValidateEventVisibilityAsync(instance, userId, cancellationToken);

            var oldEvent = await Repo.GetWithNoTrackingAsync(instance.Id, cancellationToken);

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

                var oldLocalDate = await oldEvent.GetLocalEventTime(mapManager);
                var newLocalDate = await instance.GetLocalEventTime(mapManager);

                emailCopy = emailCopy.Replace("{EventDate}", oldLocalDate.Date);
                emailCopy = emailCopy.Replace("{EventTime}", oldLocalDate.Time);

                var subject = "A TrashMob.eco event you were scheduled to attend has been updated!";

                var eventAttendees = eventAttendeeRepository.Get(m => m.EventId == instance.Id).Include(a => a.User);

                foreach (var attendee in eventAttendees)
                {
                    var dynamicTemplateData = new
                    {
                        username = attendee.User.UserName,
                        eventName = instance.Name,
                        eventDate = newLocalDate.Date,
                        eventTime = newLocalDate.Time,
                        eventAddress = instance.EventAddress(),
                        emailCopy,
                        subject,
                        eventDetailsUrl = instance.EventDetailsUrl(),
                        googleMapsUrl = instance.GoogleMapsUrl(),
                    };

                    List<EmailAddress> recipients =
                    [
                        new() { Name = attendee.User.UserName, Email = attendee.User.Email },
                    ];

                    await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.EventEmail,
                            SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None);
                }
            }

            return updatedEvent;
        }

        /// <inheritdoc />
        public async Task<Event> AddInstantEventAsync(double latitude, double longitude, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var localTimestamp = now.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

            var instantEvent = new Event
            {
                Name = $"Instant Event – {localTimestamp}",
                Description = "Instant private event",
                EventDate = now,
                DurationHours = 0,
                DurationMinutes = 0,
                EventTypeId = DefaultEventTypeIdCleanup,
                EventStatusId = (int)EventStatusEnum.Active,
                EventVisibilityId = (int)EventVisibilityEnum.Private,
                Latitude = latitude,
                Longitude = longitude,
                MaxNumberOfParticipants = 1,
            };

            // Reverse-geocode server-side (Project 65 Phase 2). Populates the address
            // fields so the event has a human-readable location for maps + community
            // stats without requiring the mobile client to do a follow-up UpdateEvent.
            // Geocode failures (network, quota, invalid coordinates) are logged rather
            // than propagated — an event with only GPS is still valid; missing address
            // fields will just render as coordinates. Runs synchronously because the
            // mobile client is still on the "Starting your pick…" status message during
            // this call; typical Azure Maps latency is well under a second.
            await PopulateAddressFromGpsAsync(instantEvent);

            // Bypass this.AddAsync — that override sends a "new event alert" email to
            // info@trashmob.eco which is spam for solo private cleanups. Call base directly
            // to persist the event, then wire up the creator-as-lead attendee ourselves.
            var newEvent = await base.AddAsync(instantEvent, userId, cancellationToken);

            var newEventAttendee = new EventAttendee
            {
                UserId = userId,
                EventId = newEvent.Id,
                SignUpDate = DateTime.UtcNow,
                IsEventLead = true,
            };

            await eventAttendeeManager.AddAsync(newEventAttendee, userId, cancellationToken);

            return newEvent;
        }

        /// <inheritdoc />
        public async Task<Event> CompleteEventAsync(Guid eventId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var mobEvent = await Repo.GetAsync(eventId, cancellationToken)
                ?? throw new InvalidOperationException($"Event {eventId} not found.");

            var elapsed = DateTimeOffset.UtcNow - mobEvent.EventDate;

            // Clamp to [0, 24h]. Negative can happen if EventDate was backdated to the future
            // (weird but possible via manual DB edits). 24h cap prevents an abandoned Instant
            // Event that never got Stopped from returning a comically long duration when the
            // background-abandonment job (Phase 4) eventually completes it.
            if (elapsed < TimeSpan.Zero)
            {
                elapsed = TimeSpan.Zero;
            }
            else if (elapsed > TimeSpan.FromHours(24))
            {
                elapsed = TimeSpan.FromHours(24);
            }

            mobEvent.DurationHours = (int)elapsed.TotalHours;
            mobEvent.DurationMinutes = elapsed.Minutes;
            mobEvent.EventStatusId = (int)EventStatusEnum.Complete;

            return await base.UpdateAsync(mobEvent, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> CompleteAbandonedInstantEventsAsync(TimeSpan abandonmentThreshold,
            CancellationToken cancellationToken = default)
        {
            var cutoff = DateTimeOffset.UtcNow - abandonmentThreshold;

            var abandoned = await Repo.Get(e =>
                    e.EventStatusId == (int)EventStatusEnum.Active
                    && e.EventVisibilityId == (int)EventVisibilityEnum.Private
                    && e.DurationHours == 0
                    && e.DurationMinutes == 0
                    && e.EventDate < cutoff)
                .ToListAsync(cancellationToken);

            if (abandoned.Count == 0)
            {
                return 0;
            }

            // Duration is set to the threshold rather than the true elapsed time. The
            // user was probably done long before the "abandoned" line — we don't know
            // when, so pick a plausible cap. Also keeps abandoned events from returning
            // absurd durations (e.g. an event started three days ago shouldn't show as
            // 72h of cleanup).
            var thresholdHours = (int)abandonmentThreshold.TotalHours;
            var thresholdMinutes = abandonmentThreshold.Minutes;

            foreach (var mobEvent in abandoned)
            {
                mobEvent.EventStatusId = (int)EventStatusEnum.Complete;
                mobEvent.DurationHours = thresholdHours;
                mobEvent.DurationMinutes = thresholdMinutes;

                // Use the event creator for audit fields — never invent a sentinel
                // system-user id (Project 64 lessons learned). Semantic reading:
                // "the user's own event was auto-completed on their behalf."
                await base.UpdateAsync(mobEvent, mobEvent.CreatedByUserId, cancellationToken);
            }

            return abandoned.Count;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetInProgressInstantEventsAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Signal for "an Instant Event that never got Stopped":
            //   - Owned by the caller
            //   - Status Active (never transitioned to Complete)
            //   - Visibility Private (Instant Events are always Private)
            //   - Duration = 0h 0m (Complete would have set it from elapsed time)
            //   - Started in the last 24 hours (older ones are abandonment cases
            //     Phase 4 will auto-complete server-side rather than resume)
            //
            // Deliberately no filter on Name — the "Instant Event – <timestamp>" prefix
            // is a display convention, and coupling this query to a string format that
            // could change is more brittle than the semantic filter above.
            var cutoff = DateTimeOffset.UtcNow.AddHours(-24);
            return await Repo.Get(e =>
                    e.CreatedByUserId == userId
                    && e.EventStatusId == (int)EventStatusEnum.Active
                    && e.EventVisibilityId == (int)EventVisibilityEnum.Private
                    && e.DurationHours == 0
                    && e.DurationMinutes == 0
                    && e.EventDate >= cutoff)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync(cancellationToken);
        }

        private async Task PopulateAddressFromGpsAsync(Event mobEvent)
        {
            if (mobEvent.Latitude is not { } lat || mobEvent.Longitude is not { } lng)
            {
                return;
            }

            try
            {
                var address = await mapManager.GetAddressAsync(lat, lng);
                if (address == null)
                {
                    return;
                }

                mobEvent.StreetAddress = address.StreetAddress;
                mobEvent.City = address.City;
                mobEvent.Region = address.Region;
                mobEvent.Country = address.Country;
                mobEvent.PostalCode = address.PostalCode;
            }
            catch (Exception)
            {
                // Deliberately swallow — a missing address is a nice-to-have miss, not a
                // reason to fail the event creation. Exception details flow through the
                // caller's telemetry (ILogger / Application Insights) via the OpenTelemetry
                // pipeline on the HttpClient inside MapManager itself.
            }
        }

        private async Task<List<Guid>> GetUserTeamIdsAsync(Guid? userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                return [];
            }

            var teams = await teamManager.GetTeamsByUserAsync(userId.Value, cancellationToken);
            return teams.Select(t => t.Id).ToList();
        }

        private async Task ValidateEventVisibilityAsync(Event instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (instance.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly)
            {
                if (instance.TeamId == null)
                {
                    throw new InvalidOperationException("TeamId is required for team-only events.");
                }

                var teams = await teamManager.GetTeamsByUserAsync(userId, cancellationToken);
                if (!teams.Any(t => t.Id == instance.TeamId.Value))
                {
                    throw new InvalidOperationException(
                        "User must be a member of the specified team to create team-only events.");
                }
            }
            else
            {
                instance.TeamId = null;
            }
        }
    }
}