namespace TrashMob.Shared.Tests.Managers.Partners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Partners;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class PartnerLocationManagerTests
    {
        private readonly Mock<IKeyedRepository<PartnerLocation>> _partnerLocationRepository;
        private readonly PartnerLocationManager _sut;

        public PartnerLocationManagerTests()
        {
            _partnerLocationRepository = new Mock<IKeyedRepository<PartnerLocation>>();
            _sut = new PartnerLocationManager(_partnerLocationRepository.Object);
        }

        #region GetByParentIdAsync

        [Fact]
        public async Task GetByParentIdAsync_ReturnsLocationsForPartner()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var location1 = CreatePartnerLocation(partnerId, "Location 1");
            var location2 = CreatePartnerLocation(partnerId, "Location 2");
            var otherPartnerLocation = CreatePartnerLocation(Guid.NewGuid(), "Other Location");

            var locations = new List<PartnerLocation> { location1, location2, otherPartnerLocation };
            _partnerLocationRepository.SetupGet(locations);

            // Act
            var result = await _sut.GetByParentIdAsync(partnerId, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, l => Assert.Equal(partnerId, l.PartnerId));
        }

        [Fact]
        public async Task GetByParentIdAsync_ReturnsEmptyWhenNoLocations()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var otherPartnerLocation = CreatePartnerLocation(Guid.NewGuid(), "Other Location");

            _partnerLocationRepository.SetupGet(new List<PartnerLocation> { otherPartnerLocation });

            // Act
            var result = await _sut.GetByParentIdAsync(partnerId, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetPartnerForLocationAsync

        [Fact]
        public async Task GetPartnerForLocationAsync_ReturnsPartner()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .WithName("Test Partner")
                .AsActive()
                .Build();
            var location = CreatePartnerLocation(partner.Id, "Test Location");
            location.Partner = partner;

            _partnerLocationRepository.SetupGetWithFilter(new List<PartnerLocation> { location });

            // Act
            var result = await _sut.GetPartnerForLocationAsync(location.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Partner", result.Name);
        }

        #endregion

        #region GetNearbyPartnersAsync

        [Fact]
        public async Task GetNearbyPartnersAsync_ReturnsPartnersWithinRadius()
        {
            // Arrange
            var partner1 = new PartnerBuilder()
                .WithName("Seattle Partner")
                .AsActive()
                .Build();
            var partner2 = new PartnerBuilder()
                .WithName("Portland Partner")
                .AsActive()
                .Build();

            // Seattle location
            var seattleLocation = CreatePartnerLocation(partner1.Id, "Seattle Office");
            seattleLocation.Latitude = 47.6062;
            seattleLocation.Longitude = -122.3321;
            seattleLocation.IsActive = true;
            seattleLocation.Partner = partner1;

            // Portland location (~145 miles from Seattle)
            var portlandLocation = CreatePartnerLocation(partner2.Id, "Portland Office");
            portlandLocation.Latitude = 45.5152;
            portlandLocation.Longitude = -122.6784;
            portlandLocation.IsActive = true;
            portlandLocation.Partner = partner2;

            _partnerLocationRepository.SetupGetWithFilter(new List<PartnerLocation> { seattleLocation, portlandLocation });

            // Act - 50 mile radius from Seattle
            var result = await _sut.GetNearbyPartnersAsync(47.6062, -122.3321, 50, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Seattle Partner", resultList[0].Name);
        }

        [Fact]
        public async Task GetNearbyPartnersAsync_ReturnsOnlyActivePartnersWithActiveLocations()
        {
            // Arrange
            var activePartner = new PartnerBuilder()
                .WithName("Active Partner")
                .AsActive()
                .Build();
            var inactivePartner = new PartnerBuilder()
                .WithName("Inactive Partner")
                .AsInactive()
                .Build();

            var activeLocation = CreatePartnerLocation(activePartner.Id, "Active Location");
            activeLocation.Latitude = 47.6062;
            activeLocation.Longitude = -122.3321;
            activeLocation.IsActive = true;
            activeLocation.Partner = activePartner;

            var inactiveLocation = CreatePartnerLocation(activePartner.Id, "Inactive Location");
            inactiveLocation.Latitude = 47.6062;
            inactiveLocation.Longitude = -122.3321;
            inactiveLocation.IsActive = false;
            inactiveLocation.Partner = activePartner;

            var inactivePartnerLocation = CreatePartnerLocation(inactivePartner.Id, "Location");
            inactivePartnerLocation.Latitude = 47.6062;
            inactivePartnerLocation.Longitude = -122.3321;
            inactivePartnerLocation.IsActive = true;
            inactivePartnerLocation.Partner = inactivePartner;

            _partnerLocationRepository.SetupGetWithFilter(new List<PartnerLocation>
            {
                activeLocation, inactiveLocation, inactivePartnerLocation
            });

            // Act
            var result = await _sut.GetNearbyPartnersAsync(47.6062, -122.3321, 50, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Active Partner", resultList[0].Name);
        }

        [Fact]
        public async Task GetNearbyPartnersAsync_ReturnsDistinctPartners()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .WithName("Multi-Location Partner")
                .AsActive()
                .Build();

            var location1 = CreatePartnerLocation(partner.Id, "Location 1");
            location1.Latitude = 47.6062;
            location1.Longitude = -122.3321;
            location1.IsActive = true;
            location1.Partner = partner;

            var location2 = CreatePartnerLocation(partner.Id, "Location 2");
            location2.Latitude = 47.6100;
            location2.Longitude = -122.3400;
            location2.IsActive = true;
            location2.Partner = partner;

            _partnerLocationRepository.SetupGetWithFilter(new List<PartnerLocation> { location1, location2 });

            // Act
            var result = await _sut.GetNearbyPartnersAsync(47.6062, -122.3321, 50, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList); // Same partner, should only appear once
        }

        [Fact]
        public async Task GetNearbyPartnersAsync_ExcludesLocationsWithoutCoordinates()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .WithName("Partner")
                .AsActive()
                .Build();

            var locationWithCoords = CreatePartnerLocation(partner.Id, "With Coords");
            locationWithCoords.Latitude = 47.6062;
            locationWithCoords.Longitude = -122.3321;
            locationWithCoords.IsActive = true;
            locationWithCoords.Partner = partner;

            var locationWithoutCoords = CreatePartnerLocation(partner.Id, "Without Coords");
            locationWithoutCoords.Latitude = null;
            locationWithoutCoords.Longitude = null;
            locationWithoutCoords.IsActive = true;
            locationWithoutCoords.Partner = partner;

            _partnerLocationRepository.SetupGetWithFilter(new List<PartnerLocation>
            {
                locationWithCoords, locationWithoutCoords
            });

            // Act
            var result = await _sut.GetNearbyPartnersAsync(47.6062, -122.3321, 50, CancellationToken.None);

            // Assert - should find partner via the location with coords
            Assert.Single(result);
        }

        #endregion

        private static PartnerLocation CreatePartnerLocation(Guid partnerId, string name)
        {
            var creatorId = Guid.NewGuid();
            return new PartnerLocation
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Name = name,
                IsActive = true,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                PartnerLocationContacts = new List<PartnerLocationContact>()
            };
        }
    }
}
