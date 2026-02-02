namespace TrashMob.Shared.Managers.Communities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for community page operations.
    /// Communities are partners with enabled home pages.
    /// </summary>
    public class CommunityManager : ICommunityManager
    {
        private readonly IKeyedRepository<Partner> partnerRepository;
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IKeyedRepository<Team> teamRepository;
        private readonly IKeyedRepository<LitterReport> litterReportRepository;
        private readonly IKeyedRepository<LitterImage> litterImageRepository;
        private readonly IBaseRepository<EventSummary> eventSummaryRepository;
        private const int CancelledEventStatusId = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityManager"/> class.
        /// </summary>
        /// <param name="partnerRepository">The partner repository.</param>
        /// <param name="eventRepository">The event repository.</param>
        /// <param name="teamRepository">The team repository.</param>
        /// <param name="litterReportRepository">The litter report repository.</param>
        /// <param name="litterImageRepository">The litter image repository.</param>
        /// <param name="eventSummaryRepository">The event summary repository.</param>
        public CommunityManager(
            IKeyedRepository<Partner> partnerRepository,
            IKeyedRepository<Event> eventRepository,
            IKeyedRepository<Team> teamRepository,
            IKeyedRepository<LitterReport> litterReportRepository,
            IKeyedRepository<LitterImage> litterImageRepository,
            IBaseRepository<EventSummary> eventSummaryRepository)
        {
            this.partnerRepository = partnerRepository;
            this.eventRepository = eventRepository;
            this.teamRepository = teamRepository;
            this.litterReportRepository = litterReportRepository;
            this.litterImageRepository = litterImageRepository;
            this.eventSummaryRepository = eventSummaryRepository;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Partner>> GetEnabledCommunitiesAsync(
            double? latitude = null,
            double? longitude = null,
            double? radiusMiles = null,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var communities = await partnerRepository.Get()
                .Where(p => p.HomePageEnabled
                    && (p.HomePageStartDate == null || p.HomePageStartDate <= now)
                    && (p.HomePageEndDate == null || p.HomePageEndDate >= now)
                    && p.PartnerStatusId == (int)PartnerStatusEnum.Active)
                .ToListAsync(cancellationToken);

            if (latitude.HasValue && longitude.HasValue && radiusMiles.HasValue)
            {
                communities = communities
                    .Where(c => c.Latitude.HasValue && c.Longitude.HasValue &&
                                CalculateDistance(latitude.Value, longitude.Value, c.Latitude.Value, c.Longitude.Value) <= radiusMiles.Value)
                    .ToList();
            }

            return communities;
        }

        /// <inheritdoc />
        public async Task<Partner> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var normalizedSlug = slug.Trim().ToLowerInvariant();
            var now = DateTimeOffset.UtcNow;

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            return await partnerRepository.Get()
                .FirstOrDefaultAsync(p => p.Slug.ToLower() == normalizedSlug
                    && p.HomePageEnabled
                    && (p.HomePageStartDate == null || p.HomePageStartDate <= now)
                    && (p.HomePageEndDate == null || p.HomePageEndDate >= now)
                    && p.PartnerStatusId == (int)PartnerStatusEnum.Active,
                    cancellationToken);
#pragma warning restore CA1862
        }

        /// <inheritdoc />
        public async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludePartnerId = null, CancellationToken cancellationToken = default)
        {
            var normalizedSlug = slug.Trim().ToLowerInvariant();

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            var query = partnerRepository.Get()
                .Where(p => p.Slug.ToLower() == normalizedSlug);
#pragma warning restore CA1862

            if (excludePartnerId.HasValue)
            {
                query = query.Where(p => p.Id != excludePartnerId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Calculates the distance between two geographic coordinates using the Haversine formula.
        /// </summary>
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 3959; // Earth's radius in miles

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default)
        {
            var community = await GetBySlugAsync(slug, cancellationToken);
            if (community == null || string.IsNullOrWhiteSpace(community.City) || string.IsNullOrWhiteSpace(community.Region))
            {
                return Enumerable.Empty<Event>();
            }

            var now = DateTimeOffset.UtcNow;
            var query = eventRepository.Get()
                .Where(e => e.City == community.City
                    && e.Region == community.Region
                    && e.EventStatusId != CancelledEventStatusId);

            if (upcomingOnly)
            {
                query = query.Where(e => e.EventDate >= now);
            }

            return await query.OrderBy(e => e.EventDate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Team>> GetCommunityTeamsAsync(string slug, double radiusMiles = 50, CancellationToken cancellationToken = default)
        {
            var community = await GetBySlugAsync(slug, cancellationToken);
            if (community == null || !community.Latitude.HasValue || !community.Longitude.HasValue)
            {
                return Enumerable.Empty<Team>();
            }

            var teams = await teamRepository.Get()
                .Where(t => t.IsPublic && t.IsActive && t.Latitude.HasValue && t.Longitude.HasValue)
                .ToListAsync(cancellationToken);

            return teams
                .Where(t => CalculateDistance(community.Latitude.Value, community.Longitude.Value, t.Latitude.Value, t.Longitude.Value) <= radiusMiles)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LitterReport>> GetCommunityLitterReportsAsync(string slug, CancellationToken cancellationToken = default)
        {
            var community = await GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return Enumerable.Empty<LitterReport>();
            }

            // Get litter reports by matching city/region from the litter images
            var litterImages = await litterImageRepository.Get()
                .Include(li => li.LitterReport)
                .Where(li => li.City == community.City && li.Region == community.Region && !li.IsCancelled)
                .ToListAsync(cancellationToken);

            // Get unique litter reports from the images
            var litterReportIds = litterImages
                .Where(li => li.LitterReport != null)
                .Select(li => li.LitterReportId)
                .Distinct()
                .ToList();

            return await litterReportRepository.Get()
                .Where(lr => litterReportIds.Contains(lr.Id) && lr.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
                .Include(lr => lr.LitterImages)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Stats> GetCommunityStatsAsync(string slug, CancellationToken cancellationToken = default)
        {
            var stats = new Stats();

            var community = await GetBySlugAsync(slug, cancellationToken);
            if (community == null || string.IsNullOrWhiteSpace(community.City) || string.IsNullOrWhiteSpace(community.Region))
            {
                return stats;
            }

            // Get events in this community
            var events = await eventRepository.Get()
                .Where(e => e.City == community.City
                    && e.Region == community.Region
                    && e.EventStatusId != CancelledEventStatusId)
                .ToListAsync(cancellationToken);

            stats.TotalEvents = events.Count;

            // Get event summaries for these events
            var eventIds = events.Select(e => e.Id).ToList();
            var eventSummaries = await eventSummaryRepository.Get()
                .Where(es => eventIds.Contains(es.EventId))
                .ToListAsync(cancellationToken);

            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) + eventSummaries.Sum(es => es.NumberOfBuckets) / 3;
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes * es.ActualNumberOfAttendees / 60);
            stats.TotalParticipants = eventSummaries.Sum(es => es.ActualNumberOfAttendees);
            stats.TotalWeightInPounds = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => e.PickedWeight) +
                                        eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => e.PickedWeight * 2.20462m);
            stats.TotalWeightInKilograms = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => e.PickedWeight) +
                                           eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => e.PickedWeight * 0.453592m);

            // Get litter reports in the community
            var litterReports = await GetCommunityLitterReportsAsync(slug, cancellationToken);
            stats.TotalLitterReportsSubmitted = litterReports.Count();
            stats.TotalLitterReportsClosed = litterReports.Count(lr => lr.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned);

            return stats;
        }
    }
}
