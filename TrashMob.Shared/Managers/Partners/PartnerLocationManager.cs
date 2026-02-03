namespace TrashMob.Shared.Managers.Partners
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

    /// <summary>
    /// Manages partner locations including CRUD operations and retrieving locations with their contacts.
    /// </summary>
    public class PartnerLocationManager : KeyedManager<PartnerLocation>, IPartnerLocationManager
    {
        private const double EarthRadiusMiles = 3959.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocationManager"/> class.
        /// </summary>
        /// <param name="partnerLocationRepository">The repository for partner location data access.</param>
        public PartnerLocationManager(IKeyedRepository<PartnerLocation> partnerLocationRepository) : base(
            partnerLocationRepository)
        {
        }

        /// <inheritdoc />
        public async Task<Partner> GetPartnerForLocationAsync(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partnerLocation = await Repository.Get(pl => pl.Id == partnerLocationId)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(cancellationToken);

            return partnerLocation.Partner;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerLocation>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).Include(p => p.PartnerLocationContacts)
                .ToListAsync(cancellationToken)).AsEnumerable();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Partner>> GetNearbyPartnersAsync(double latitude, double longitude, double radiusMiles, CancellationToken cancellationToken)
        {
            // Get all active locations with coordinates and their partners
            var locations = await Repository.Get(pl => pl.IsActive && pl.Latitude.HasValue && pl.Longitude.HasValue)
                .Include(pl => pl.Partner)
                .Where(pl => pl.Partner.PartnerStatusId == (int)PartnerStatusEnum.Active)
                .ToListAsync(cancellationToken);

            // Filter by distance using Haversine formula in memory
            var nearbyLocations = locations
                .Select(loc => new
                {
                    Location = loc,
                    Distance = CalculateDistanceMiles(latitude, longitude, loc.Latitude!.Value, loc.Longitude!.Value),
                })
                .Where(x => x.Distance <= radiusMiles)
                .OrderBy(x => x.Distance)
                .Select(x => x.Location)
                .ToList();

            // Return distinct partners
            return nearbyLocations
                .Select(l => l.Partner)
                .DistinctBy(p => p.Id)
                .ToList();
        }

        /// <summary>
        /// Calculates the distance between two geographic points using the Haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of the first point.</param>
        /// <param name="lon1">Longitude of the first point.</param>
        /// <param name="lat2">Latitude of the second point.</param>
        /// <param name="lon2">Longitude of the second point.</param>
        /// <returns>Distance in miles.</returns>
        private static double CalculateDistanceMiles(double lat1, double lon1, double lat2, double lon2)
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
