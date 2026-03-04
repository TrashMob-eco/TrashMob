namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    public class EventDependentManager(
        IKeyedRepository<EventDependent> repository,
        IKeyedRepository<Dependent> dependentRepository,
        IDependentWaiverManager dependentWaiverManager,
        IEventAttendeeManager eventAttendeeManager)
        : KeyedManager<EventDependent>(repository), IEventDependentManager
    {
        public async Task<ServiceResult<IEnumerable<EventDependent>>> RegisterDependentsAsync(
            Guid eventId,
            IEnumerable<Guid> dependentIds,
            Guid parentUserId,
            CancellationToken cancellationToken = default)
        {
            // Validate parent is registered as attendee for this event
            var attendees = await eventAttendeeManager
                .GetAsync(ea => ea.EventId == eventId && ea.UserId == parentUserId, cancellationToken);
            if (!attendees.Any())
            {
                return ServiceResult<IEnumerable<EventDependent>>.Failure(
                    "You must be registered as an attendee before adding dependents to this event.");
            }

            var results = new List<EventDependent>();

            foreach (var dependentId in dependentIds)
            {
                // Validate dependent belongs to parent and is active
                var dependent = await dependentRepository.GetAsync(dependentId, cancellationToken);
                if (dependent == null || dependent.ParentUserId != parentUserId || !dependent.IsActive)
                {
                    return ServiceResult<IEnumerable<EventDependent>>.Failure(
                        $"Dependent {dependentId} not found or does not belong to you.");
                }

                // Check for existing registration
                var existing = await Repo.Get(ed => ed.EventId == eventId && ed.DependentId == dependentId)
                    .AnyAsync(cancellationToken);
                if (existing)
                {
                    continue; // Already registered, skip
                }

                // Validate valid waiver exists
                var currentWaiver = await dependentWaiverManager.GetCurrentWaiverAsync(dependentId, cancellationToken);
                if (currentWaiver == null)
                {
                    return ServiceResult<IEnumerable<EventDependent>>.Failure(
                        $"Dependent '{dependent.FirstName} {dependent.LastName}' does not have a valid waiver. Please sign a waiver first.");
                }

                var eventDependent = new EventDependent
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    DependentId = dependentId,
                    ParentUserId = parentUserId,
                    DependentWaiverId = currentWaiver.Id,
                };

                var created = await AddAsync(eventDependent, parentUserId, cancellationToken);
                results.Add(created);
            }

            return ServiceResult<IEnumerable<EventDependent>>.Success(results);
        }

        public async Task<IEnumerable<EventDependent>> GetByEventIdAsync(
            Guid eventId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(ed => ed.EventId == eventId)
                .Include(ed => ed.Dependent)
                .Include(ed => ed.ParentUser)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetDependentCountAsync(
            Guid eventId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(ed => ed.EventId == eventId)
                .CountAsync(cancellationToken);
        }

        public async Task<int> UnregisterDependentAsync(
            Guid eventId, Guid dependentId, Guid userId, CancellationToken cancellationToken = default)
        {
            var eventDependent = await Repo.Get(ed => ed.EventId == eventId && ed.DependentId == dependentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (eventDependent == null)
            {
                return 0;
            }

            if (eventDependent.ParentUserId != userId)
            {
                throw new InvalidOperationException("Only the parent/guardian can unregister a dependent.");
            }

            return await Repository.DeleteAsync(eventDependent);
        }
    }
}
