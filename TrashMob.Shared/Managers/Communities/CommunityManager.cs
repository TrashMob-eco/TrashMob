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
    public class CommunityManager(
        IKeyedRepository<Partner> partnerRepository,
        IKeyedRepository<Event> eventRepository,
        IKeyedRepository<Team> teamRepository,
        IKeyedRepository<LitterReport> litterReportRepository,
        IKeyedRepository<LitterImage> litterImageRepository,
        IBaseRepository<EventSummary> eventSummaryRepository)
        : ICommunityManager
    {
        private const int CancelledEventStatusId = 3;

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

        /// <summary>
        /// Determines whether a community uses bounding-box filtering instead of exact city/region match.
        /// County, state, and regional communities with defined bounds use bounding-box filtering.
        /// </summary>
        private static bool UsesBoundingBoxFilter(Partner community)
        {
            return community.RegionType.HasValue
                && community.RegionType.Value != (int)RegionTypeEnum.City
                && community.BoundsNorth.HasValue && community.BoundsSouth.HasValue
                && community.BoundsEast.HasValue && community.BoundsWest.HasValue;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default)
        {
            var community = await GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return Enumerable.Empty<Event>();
            }

            IQueryable<Event> query;

            if (UsesBoundingBoxFilter(community))
            {
                query = eventRepository.Get()
                    .Where(e => e.Latitude >= community.BoundsSouth.Value
                        && e.Latitude <= community.BoundsNorth.Value
                        && e.Longitude >= community.BoundsWest.Value
                        && e.Longitude <= community.BoundsEast.Value
                        && e.EventStatusId != CancelledEventStatusId);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(community.City) || string.IsNullOrWhiteSpace(community.Region))
                {
                    return Enumerable.Empty<Event>();
                }

                query = eventRepository.Get()
                    .Where(e => e.City == community.City
                        && e.Region == community.Region
                        && e.EventStatusId != CancelledEventStatusId);
            }

            var now = DateTimeOffset.UtcNow;

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

            IQueryable<LitterImage> litterImageQuery;

            if (UsesBoundingBoxFilter(community))
            {
                litterImageQuery = litterImageRepository.Get()
                    .Include(li => li.LitterReport)
                    .Where(li => li.Latitude.HasValue && li.Longitude.HasValue
                        && li.Latitude.Value >= community.BoundsSouth.Value
                        && li.Latitude.Value <= community.BoundsNorth.Value
                        && li.Longitude.Value >= community.BoundsWest.Value
                        && li.Longitude.Value <= community.BoundsEast.Value
                        && !li.IsCancelled);
            }
            else
            {
                litterImageQuery = litterImageRepository.Get()
                    .Include(li => li.LitterReport)
                    .Where(li => li.City == community.City && li.Region == community.Region && !li.IsCancelled);
            }

            var litterImages = await litterImageQuery.ToListAsync(cancellationToken);

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
            if (community == null)
            {
                return stats;
            }

            IQueryable<Event> eventQuery;

            if (UsesBoundingBoxFilter(community))
            {
                eventQuery = eventRepository.Get()
                    .Where(e => e.Latitude >= community.BoundsSouth.Value
                        && e.Latitude <= community.BoundsNorth.Value
                        && e.Longitude >= community.BoundsWest.Value
                        && e.Longitude <= community.BoundsEast.Value
                        && e.EventStatusId != CancelledEventStatusId);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(community.City) || string.IsNullOrWhiteSpace(community.Region))
                {
                    return stats;
                }

                eventQuery = eventRepository.Get()
                    .Where(e => e.City == community.City
                        && e.Region == community.Region
                        && e.EventStatusId != CancelledEventStatusId);
            }

            // Get events in this community
            var events = await eventQuery.ToListAsync(cancellationToken);

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

        /// <inheritdoc />
        public async Task<IEnumerable<Partner>> GetFeaturedCommunitiesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            return await partnerRepository.Get()
                .Where(p => p.HomePageEnabled
                    && p.IsFeatured
                    && (p.HomePageStartDate == null || p.HomePageStartDate <= now)
                    && (p.HomePageEndDate == null || p.HomePageEndDate >= now)
                    && p.PartnerStatusId == (int)PartnerStatusEnum.Active)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CommunityPublicStats> GetPublicStatsAsync(CancellationToken cancellationToken = default)
        {
            var communities = await GetEnabledCommunitiesAsync(cancellationToken: cancellationToken);
            var communityList = communities.ToList();

            var stats = new CommunityPublicStats
            {
                TotalCommunities = communityList.Count,
            };

            // Aggregate events and volunteers across all communities
            foreach (var community in communityList)
            {
                if (string.IsNullOrWhiteSpace(community.Slug))
                {
                    continue;
                }

                var communityStats = await GetCommunityStatsAsync(community.Slug, cancellationToken);
                stats.TotalCommunityEvents += communityStats.TotalEvents;
                stats.TotalCommunityVolunteers += communityStats.TotalParticipants;
            }

            return stats;
        }

        /// <inheritdoc />
        public async Task<Partner> GetByIdAsync(Guid partnerId, CancellationToken cancellationToken = default)
        {
            return await partnerRepository.GetAsync(partnerId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Partner> UpdateCommunityContentAsync(Partner community, Guid userId, CancellationToken cancellationToken = default)
        {
            var existing = await partnerRepository.GetAsync(community.Id, cancellationToken);
            if (existing == null)
            {
                return null;
            }

            // Only update fields that community admins are allowed to edit
            existing.PublicNotes = community.PublicNotes;
            existing.Tagline = community.Tagline;
            existing.BrandingPrimaryColor = community.BrandingPrimaryColor;
            existing.BrandingSecondaryColor = community.BrandingSecondaryColor;
            existing.BannerImageUrl = community.BannerImageUrl;
            existing.LogoUrl = community.LogoUrl;
            existing.ContactEmail = community.ContactEmail;
            existing.ContactPhone = community.ContactPhone;
            existing.PhysicalAddress = community.PhysicalAddress;
            existing.Website = community.Website;
            existing.RegionType = community.RegionType;
            existing.CountyName = community.CountyName;
            existing.BoundsNorth = community.BoundsNorth;
            existing.BoundsSouth = community.BoundsSouth;
            existing.BoundsEast = community.BoundsEast;
            existing.BoundsWest = community.BoundsWest;
            existing.DefaultCleanupFrequencyDays = community.DefaultCleanupFrequencyDays;
            existing.DefaultMinEventsPerYear = community.DefaultMinEventsPerYear;
            existing.DefaultSafetyRequirements = community.DefaultSafetyRequirements;
            existing.DefaultAllowCoAdoption = community.DefaultAllowCoAdoption;
            existing.LastUpdatedByUserId = userId;
            existing.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await partnerRepository.UpdateAsync(existing);
        }

        /// <inheritdoc />
        public async Task<CommunityDashboard> GetCommunityDashboardAsync(Guid partnerId, CancellationToken cancellationToken = default)
        {
            var community = await partnerRepository.GetAsync(partnerId, cancellationToken);
            if (community == null)
            {
                return null;
            }

            var dashboard = new CommunityDashboard
            {
                Community = community,
                Stats = new Stats(),
                RecentEvents = [],
                UpcomingEvents = [],
                RecentActivity = [],
            };

            // Get stats if we have city/region
            if (!string.IsNullOrWhiteSpace(community.Slug))
            {
                dashboard.Stats = await GetCommunityStatsAsync(community.Slug, cancellationToken);
            }

            // Get events in this community
            var hasBoundsFilter = UsesBoundingBoxFilter(community);
            var hasCityFilter = !string.IsNullOrWhiteSpace(community.City) && !string.IsNullOrWhiteSpace(community.Region);

            if (hasBoundsFilter || hasCityFilter)
            {
                var now = DateTimeOffset.UtcNow;
                var thirtyDaysAgo = now.AddDays(-30);

                IQueryable<Event> baseEventQuery;

                if (hasBoundsFilter)
                {
                    baseEventQuery = eventRepository.Get()
                        .Where(e => e.Latitude >= community.BoundsSouth.Value
                            && e.Latitude <= community.BoundsNorth.Value
                            && e.Longitude >= community.BoundsWest.Value
                            && e.Longitude <= community.BoundsEast.Value
                            && e.EventStatusId != CancelledEventStatusId);
                }
                else
                {
                    baseEventQuery = eventRepository.Get()
                        .Where(e => e.City == community.City
                            && e.Region == community.Region
                            && e.EventStatusId != CancelledEventStatusId);
                }

                // Upcoming events
                var upcomingEvents = await baseEventQuery
                    .Where(e => e.EventDate >= now)
                    .OrderBy(e => e.EventDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);
                dashboard.UpcomingEvents = upcomingEvents;

                // Recent events (completed in last 30 days)
                var recentEvents = await baseEventQuery
                    .Where(e => e.EventDate < now && e.EventDate >= thirtyDaysAgo)
                    .OrderByDescending(e => e.EventDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);
                dashboard.RecentEvents = recentEvents;

                // Build recent activity from completed events
                List<CommunityActivity> recentActivity = [];
                foreach (var evt in recentEvents.Take(10))
                {
                    recentActivity.Add(new CommunityActivity
                    {
                        ActivityType = "EventCompleted",
                        Description = $"Event \"{evt.Name}\" completed",
                        ActivityDate = evt.EventDate,
                        RelatedEntityId = evt.Id,
                    });
                }

                dashboard.RecentActivity = recentActivity.OrderByDescending(a => a.ActivityDate).ToList();
            }

            // Get team count
            if (community.Latitude.HasValue && community.Longitude.HasValue)
            {
                var teams = await teamRepository.Get()
                    .Where(t => t.IsPublic && t.IsActive && t.Latitude.HasValue && t.Longitude.HasValue)
                    .ToListAsync(cancellationToken);

                dashboard.TeamCount = teams
                    .Count(t => CalculateDistance(community.Latitude.Value, community.Longitude.Value, t.Latitude.Value, t.Longitude.Value) <= 50);
            }

            // Get open litter reports count
            if (hasBoundsFilter || hasCityFilter)
            {
                IQueryable<LitterImage> litterQuery;

                if (hasBoundsFilter)
                {
                    litterQuery = litterImageRepository.Get()
                        .Include(li => li.LitterReport)
                        .Where(li => li.Latitude.HasValue && li.Longitude.HasValue
                            && li.Latitude.Value >= community.BoundsSouth.Value
                            && li.Latitude.Value <= community.BoundsNorth.Value
                            && li.Longitude.Value >= community.BoundsWest.Value
                            && li.Longitude.Value <= community.BoundsEast.Value
                            && !li.IsCancelled);
                }
                else
                {
                    litterQuery = litterImageRepository.Get()
                        .Include(li => li.LitterReport)
                        .Where(li => li.City == community.City && li.Region == community.Region && !li.IsCancelled);
                }

                var litterImages = await litterQuery.ToListAsync(cancellationToken);

                var openLitterReportIds = litterImages
                    .Where(li => li.LitterReport != null && li.LitterReport.LitterReportStatusId == (int)LitterReportStatusEnum.New)
                    .Select(li => li.LitterReportId)
                    .Distinct()
                    .ToList();

                dashboard.OpenLitterReportsCount = openLitterReportIds.Count;
            }

            return dashboard;
        }
    }
}
