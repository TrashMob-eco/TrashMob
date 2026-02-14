namespace TrashMob.Shared.Managers.Adoptions
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

    /// <summary>
    /// Manager for team adoption event linking operations.
    /// </summary>
    public class TeamAdoptionEventManager(
        IKeyedRepository<TeamAdoptionEvent> repository,
        ITeamAdoptionManager adoptionManager,
        IKeyedManager<Event> eventManager,
        IKeyedRepository<TeamAdoption> adoptionRepository)
        : KeyedManager<TeamAdoptionEvent>(repository), ITeamAdoptionEventManager
    {

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoptionEvent>> LinkEventAsync(
            Guid teamAdoptionId,
            Guid eventId,
            string notes,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Validate adoption exists and is approved
            var adoption = await adoptionManager.GetAsync(teamAdoptionId, cancellationToken);
            if (adoption == null)
            {
                return ServiceResult<TeamAdoptionEvent>.Failure("Adoption not found.");
            }

            if (adoption.Status != TeamAdoptionStatus.Approved)
            {
                return ServiceResult<TeamAdoptionEvent>.Failure("Only approved adoptions can have events linked.");
            }

            // Validate event exists
            var evt = await eventManager.GetAsync(eventId, cancellationToken);
            if (evt == null)
            {
                return ServiceResult<TeamAdoptionEvent>.Failure("Event not found.");
            }

            // Check if already linked
            if (await IsEventLinkedAsync(teamAdoptionId, eventId, cancellationToken))
            {
                return ServiceResult<TeamAdoptionEvent>.Failure("Event is already linked to this adoption.");
            }

            // Create the link
            var adoptionEvent = new TeamAdoptionEvent
            {
                Id = Guid.NewGuid(),
                TeamAdoptionId = teamAdoptionId,
                EventId = eventId,
                Notes = notes,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            var created = await AddAsync(adoptionEvent, userId, cancellationToken);

            // Update compliance tracking on the adoption
            await UpdateComplianceAsync(teamAdoptionId, userId, cancellationToken);

            return ServiceResult<TeamAdoptionEvent>.Success(created);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<bool>> UnlinkEventAsync(
            Guid teamAdoptionEventId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var adoptionEvent = await GetAsync(teamAdoptionEventId, cancellationToken);
            if (adoptionEvent == null)
            {
                return ServiceResult<bool>.Failure("Adoption event link not found.");
            }

            var teamAdoptionId = adoptionEvent.TeamAdoptionId;

            await DeleteAsync(teamAdoptionEventId, cancellationToken);

            // Update compliance tracking on the adoption
            await UpdateComplianceAsync(teamAdoptionId, userId, cancellationToken);

            return ServiceResult<bool>.Success(true);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoptionEvent>> GetByAdoptionIdAsync(
            Guid teamAdoptionId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(ae => ae.TeamAdoptionId == teamAdoptionId)
                .Include(ae => ae.Event)
                .OrderByDescending(ae => ae.Event.EventDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoptionEvent>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(ae => ae.EventId == eventId)
                .Include(ae => ae.TeamAdoption)
                    .ThenInclude(a => a.AdoptableArea)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> IsEventLinkedAsync(
            Guid teamAdoptionId,
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get(ae => ae.TeamAdoptionId == teamAdoptionId && ae.EventId == eventId)
                .AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetActiveAdoptionsForTeamAsync(
            Guid teamId,
            CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await adoptionRepository
                .Get(a => a.TeamId == teamId
                    && a.Status == TeamAdoptionStatus.Approved
                    && (a.AdoptionEndDate == null || a.AdoptionEndDate >= today))
                .Include(a => a.AdoptableArea)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task UpdateComplianceAsync(
            Guid teamAdoptionId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var adoption = await adoptionManager.GetAsync(teamAdoptionId, cancellationToken);
            if (adoption == null)
            {
                return;
            }

            // Get all linked events
            var linkedEvents = await GetByAdoptionIdAsync(teamAdoptionId, cancellationToken);
            var eventsList = linkedEvents.ToList();

            // Update tracking fields
            adoption.EventCount = eventsList.Count;
            adoption.LastEventDate = eventsList
                .Where(ae => ae.Event != null)
                .Select(ae => ae.Event.EventDate)
                .OrderByDescending(d => d)
                .FirstOrDefault();

            // Calculate compliance
            // A team is compliant if they have had an event within the required frequency
            adoption.IsCompliant = CalculateCompliance(adoption);

            adoption.LastUpdatedByUserId = userId;
            adoption.LastUpdatedDate = DateTimeOffset.UtcNow;

            await adoptionManager.UpdateAsync(adoption, userId, cancellationToken);
        }

        private static bool CalculateCompliance(TeamAdoption adoption)
        {
            // If no events yet, check if adoption is new enough to still be compliant
            if (!adoption.LastEventDate.HasValue)
            {
                if (!adoption.AdoptionStartDate.HasValue)
                {
                    return true; // No start date means we can't determine compliance yet
                }

                // Allow a grace period for new adoptions (90 days default)
                var daysSinceStart = (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - adoption.AdoptionStartDate.Value.DayNumber);
                return daysSinceStart <= 90; // Grace period for first event
            }

            // Check if last event was within the required frequency
            // Default cleanup frequency is 90 days if not specified on the area
            var daysSinceLastEvent = (DateTimeOffset.UtcNow - adoption.LastEventDate.Value).TotalDays;
            var requiredFrequency = adoption.AdoptableArea?.CleanupFrequencyDays ?? 90;

            // Add a 30-day grace period
            return daysSinceLastEvent <= (requiredFrequency + 30);
        }
    }
}
