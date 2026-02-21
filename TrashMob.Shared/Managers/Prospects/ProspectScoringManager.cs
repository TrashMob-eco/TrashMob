namespace TrashMob.Shared.Managers.Prospects
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
    /// Calculates FitScores for community prospects and identifies geographic gaps
    /// where events happen but no active community partner exists.
    /// </summary>
    public class ProspectScoringManager(
        IKeyedRepository<Event> eventRepository,
        IKeyedRepository<Partner> partnerRepository,
        IKeyedRepository<CommunityProspect> prospectRepository)
        : IProspectScoringManager
    {
        private const double SearchRadiusMiles = 50;
        private const double EarthRadiusMiles = 3959;

        /// <inheritdoc />
        public async Task<FitScoreBreakdown> CalculateFitScoreAsync(Guid prospectId,
            CancellationToken cancellationToken = default)
        {
            var prospect = await prospectRepository.GetAsync(prospectId, cancellationToken);

            if (prospect is null)
            {
                return null;
            }

            return await CalculateBreakdownAsync(prospect, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> RecalculateAllScoresAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var prospects = await prospectRepository.Get().ToListAsync(cancellationToken);
            var count = 0;

            foreach (var prospect in prospects)
            {
                var breakdown = await CalculateBreakdownAsync(prospect, cancellationToken);
                if (prospect.FitScore != breakdown.TotalScore)
                {
                    prospect.FitScore = breakdown.TotalScore;
                    prospect.LastUpdatedByUserId = userId;
                    prospect.LastUpdatedDate = DateTimeOffset.UtcNow;
                    await prospectRepository.UpdateAsync(prospect);
                    count++;
                }
            }

            return count;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GeographicGap>> GetGeographicGapsAsync(
            CancellationToken cancellationToken = default)
        {
            // Get all non-canceled events grouped by city/region/country
            var events = await eventRepository.Get()
                .Where(e => e.EventStatusId != (int)EventStatusEnum.Canceled)
                .ToListAsync(cancellationToken);

            var eventGroups = events
                .Where(e => !string.IsNullOrWhiteSpace(e.City))
                .GroupBy(e => new
                {
                    City = (e.City ?? string.Empty).Trim().ToLowerInvariant(),
                    Region = (e.Region ?? string.Empty).Trim().ToLowerInvariant(),
                    Country = (e.Country ?? string.Empty).Trim().ToLowerInvariant(),
                })
                .Select(g => new
                {
                    City = g.First().City,
                    Region = g.First().Region,
                    Country = g.First().Country,
                    EventCount = g.Count(),
                    AverageLatitude = g.Where(e => e.Latitude.HasValue).Select(e => e.Latitude.Value).DefaultIfEmpty(0).Average(),
                    AverageLongitude = g.Where(e => e.Longitude.HasValue).Select(e => e.Longitude.Value).DefaultIfEmpty(0).Average(),
                    HasCoordinates = g.Any(e => e.Latitude.HasValue && e.Longitude.HasValue),
                })
                .ToList();

            // Get active partners with home pages
            var activePartners = await partnerRepository.Get()
                .Where(p => p.PartnerStatusId == (int)PartnerStatusEnum.Active && p.HomePageEnabled)
                .ToListAsync(cancellationToken);

            List<GeographicGap> gaps = [];

            foreach (var group in eventGroups)
            {
                // Check if an active partner exists in this city/region
                var hasPartnerInArea = activePartners.Any(p =>
                    string.Equals(p.City?.Trim(), group.City?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(p.Region?.Trim(), group.Region?.Trim(), StringComparison.OrdinalIgnoreCase));

                if (hasPartnerInArea)
                {
                    continue;
                }

                double? nearestDistance = null;

                if (group.HasCoordinates)
                {
                    var partnersWithCoords = activePartners
                        .Where(p => p.Latitude.HasValue && p.Longitude.HasValue);

                    foreach (var partner in partnersWithCoords)
                    {
                        var dist = CalculateDistanceMiles(
                            group.AverageLatitude, group.AverageLongitude,
                            partner.Latitude!.Value, partner.Longitude!.Value);

                        if (!nearestDistance.HasValue || dist < nearestDistance.Value)
                        {
                            nearestDistance = dist;
                        }
                    }
                }

                gaps.Add(new GeographicGap
                {
                    City = group.City,
                    Region = group.Region,
                    Country = group.Country,
                    EventCount = group.EventCount,
                    NearestPartnerDistanceMiles = nearestDistance.HasValue ? Math.Round(nearestDistance.Value, 1) : null,
                    AverageLatitude = group.HasCoordinates ? Math.Round(group.AverageLatitude, 4) : null,
                    AverageLongitude = group.HasCoordinates ? Math.Round(group.AverageLongitude, 4) : null,
                });
            }

            return gaps.OrderByDescending(g => g.EventCount);
        }

        private async Task<FitScoreBreakdown> CalculateBreakdownAsync(CommunityProspect prospect,
            CancellationToken cancellationToken)
        {
            var breakdown = new FitScoreBreakdown();

            // --- Event Density (0-30) ---
            var nearbyEventCount = 0;

            if (prospect.Latitude.HasValue && prospect.Longitude.HasValue)
            {
                var allEvents = await eventRepository.Get()
                    .Where(e => e.EventStatusId != (int)EventStatusEnum.Canceled
                                && e.Latitude.HasValue && e.Longitude.HasValue)
                    .ToListAsync(cancellationToken);

                nearbyEventCount = allEvents.Count(e =>
                    CalculateDistanceMiles(prospect.Latitude.Value, prospect.Longitude.Value,
                        e.Latitude!.Value, e.Longitude!.Value) <= SearchRadiusMiles);
            }

            breakdown.NearbyEventCount = nearbyEventCount;
            breakdown.EventDensityScore = nearbyEventCount switch
            {
                0 => 0,
                <= 5 => 10,
                <= 15 => 20,
                _ => 30,
            };

            // --- Population (0-25) ---
            breakdown.PopulationScore = prospect.Population switch
            {
                null => 10,
                < 10_000 => 5,
                < 50_000 => 15,
                < 200_000 => 20,
                _ => 25,
            };

            // --- Geographic Gap (0-30) ---
            double? nearestPartnerDistance = null;

            if (prospect.Latitude.HasValue && prospect.Longitude.HasValue)
            {
                var activePartners = await partnerRepository.Get()
                    .Where(p => p.PartnerStatusId == (int)PartnerStatusEnum.Active
                                && p.HomePageEnabled
                                && p.Latitude.HasValue && p.Longitude.HasValue)
                    .ToListAsync(cancellationToken);

                foreach (var partner in activePartners)
                {
                    var dist = CalculateDistanceMiles(
                        prospect.Latitude.Value, prospect.Longitude.Value,
                        partner.Latitude!.Value, partner.Longitude!.Value);

                    if (!nearestPartnerDistance.HasValue || dist < nearestPartnerDistance.Value)
                    {
                        nearestPartnerDistance = dist;
                    }
                }
            }

            breakdown.NearestPartnerDistanceMiles = nearestPartnerDistance.HasValue
                ? Math.Round(nearestPartnerDistance.Value, 1)
                : null;

            breakdown.GeographicGapScore = nearestPartnerDistance switch
            {
                null => 30,
                >= 100 => 30,
                >= 50 => 20,
                >= 25 => 10,
                _ => 0,
            };

            // --- Community Type (0-15) ---
            breakdown.CommunityTypeFitScore = (prospect.Type?.Trim().ToLowerInvariant()) switch
            {
                "municipality" => 15,
                "nonprofit" => 12,
                "civicorg" => 10,
                "hoa" => 8,
                _ => 5,
            };

            // --- Total ---
            breakdown.TotalScore = breakdown.EventDensityScore
                                   + breakdown.PopulationScore
                                   + breakdown.GeographicGapScore
                                   + breakdown.CommunityTypeFitScore;

            return breakdown;
        }

        public static double CalculateDistanceMiles(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusMiles * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
