namespace TrashMob.Shared.Tests.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class WaiverManagerTests
    {
        private readonly Mock<IKeyedRepository<Waiver>> _waiverRepository;
        private readonly WaiverManager _sut;

        public WaiverManagerTests()
        {
            _waiverRepository = new Mock<IKeyedRepository<Waiver>>();
            _sut = new WaiverManager(_waiverRepository.Object);
        }

        #region GetByNameAsync

        [Fact]
        public async Task GetByNameAsync_ReturnsWaiverWhenFound()
        {
            // Arrange
            var waiverName = "TrashMob Waiver";
            var waiver = new Waiver
            {
                Id = Guid.NewGuid(),
                Name = waiverName,
                IsWaiverEnabled = true,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            _waiverRepository.SetupGetWithFilter(new List<Waiver> { waiver });

            // Act
            var result = await _sut.GetByNameAsync(waiverName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(waiverName, result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _waiverRepository.SetupGetWithFilter(new List<Waiver>());

            // Act
            var result = await _sut.GetByNameAsync("Nonexistent Waiver");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_MatchesExactName()
        {
            // Arrange
            var waiver1 = new Waiver
            {
                Id = Guid.NewGuid(),
                Name = "TrashMob Waiver",
                IsWaiverEnabled = true,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
            var waiver2 = new Waiver
            {
                Id = Guid.NewGuid(),
                Name = "Other Waiver",
                IsWaiverEnabled = true,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            _waiverRepository.SetupGetWithFilter(new List<Waiver> { waiver1, waiver2 });

            // Act
            var result = await _sut.GetByNameAsync("TrashMob Waiver");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TrashMob Waiver", result.Name);
        }

        #endregion
    }
}
