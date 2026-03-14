namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Contacts;
    using Xunit;

    public class DonationsV2ControllerTests
    {
        private readonly Mock<IDonationManager> donationManager = new();
        private readonly Mock<IDonationEmailManager> donationEmailManager = new();
        private readonly Mock<ILogger<DonationsV2Controller>> logger = new();
        private readonly DonationsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public DonationsV2ControllerTests()
        {
            controller = new DonationsV2Controller(
                donationManager.Object, donationEmailManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var donations = new List<Donation>
            {
                new() { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), Amount = 100m, DonationDate = DateTimeOffset.UtcNow },
                new() { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), Amount = 250m, DonationDate = DateTimeOffset.UtcNow },
            };

            donationManager
                .Setup(m => m.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(donations);

            var result = await controller.GetAll(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<DonationDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            donationManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Donation)null);

            var result = await controller.GetById(id, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var donation = new Donation { Id = id, ContactId = Guid.NewGuid(), Amount = 100m, DonationDate = DateTimeOffset.UtcNow };

            donationManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(donation);

            var result = await controller.GetById(id, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<DonationDto>(okResult.Value);
            Assert.Equal(100m, dto.Amount);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var donation = new Donation { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), Amount = 100m, DonationDate = DateTimeOffset.UtcNow };

            donationManager
                .Setup(m => m.AddAsync(It.IsAny<Donation>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(donation);

            var dto = new DonationDto { ContactId = donation.ContactId, Amount = 100m, DonationDate = DateTimeOffset.UtcNow };

            var result = await controller.Create(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<DonationDto>(createdResult.Value);
            Assert.Equal(100m, resultDto.Amount);
        }

        [Fact]
        public async Task SendThankYou_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            donationManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Donation)null);

            var result = await controller.SendThankYou(id, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
