namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class ProfessionalCleanupLogsV2ControllerTests
    {
        private readonly Mock<IProfessionalCleanupLogManager> logManager = new();
        private readonly Mock<IProfessionalCompanyManager> companyManager = new();
        private readonly Mock<ISponsoredAdoptionManager> adoptionManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<ProfessionalCleanupLogsV2Controller>> logger = new();
        private readonly ProfessionalCleanupLogsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public ProfessionalCleanupLogsV2ControllerTests()
        {
            controller = new ProfessionalCleanupLogsV2Controller(
                logManager.Object, companyManager.Object, adoptionManager.Object,
                authorizationService.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        private void SetupAuthSuccess()
        {
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
        }

        [Fact]
        public async Task GetLogs_CompanyNotFound_ReturnsNotFound()
        {
            var companyId = Guid.NewGuid();

            companyManager
                .Setup(m => m.GetAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProfessionalCompany)null);

            var result = await controller.GetLogs(companyId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetLogs_Success_ReturnsOk()
        {
            SetupAuthSuccess();

            var companyId = Guid.NewGuid();
            var company = new ProfessionalCompany { Id = companyId, Name = "Test Company", PartnerId = Guid.NewGuid(), IsActive = true };
            var logs = new List<ProfessionalCleanupLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SponsoredAdoptionId = Guid.NewGuid(),
                    ProfessionalCompanyId = companyId,
                    CleanupDate = DateTimeOffset.UtcNow,
                    DurationMinutes = 60,
                    BagsCollected = 5,
                    Notes = "Test cleanup",
                },
            };

            companyManager
                .Setup(m => m.GetAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
            logManager
                .Setup(m => m.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(logs);

            var result = await controller.GetLogs(companyId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProfessionalCleanupLogDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task LogCleanup_Success_ReturnsCreated()
        {
            SetupAuthSuccess();

            var companyId = Guid.NewGuid();
            var company = new ProfessionalCompany { Id = companyId, Name = "Test Company", PartnerId = Guid.NewGuid(), IsActive = true };
            var logDto = new ProfessionalCleanupLogDto
            {
                SponsoredAdoptionId = Guid.NewGuid(),
                CleanupDate = DateTimeOffset.UtcNow,
                DurationMinutes = 45,
                BagsCollected = 3,
                Notes = "Log cleanup test",
            };
            var createdLog = new ProfessionalCleanupLog
            {
                Id = Guid.NewGuid(),
                SponsoredAdoptionId = logDto.SponsoredAdoptionId,
                ProfessionalCompanyId = companyId,
                CleanupDate = logDto.CleanupDate,
                DurationMinutes = logDto.DurationMinutes,
                BagsCollected = logDto.BagsCollected,
                Notes = logDto.Notes,
            };

            companyManager
                .Setup(m => m.GetAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
            logManager
                .Setup(m => m.LogCleanupAsync(It.IsAny<ProfessionalCleanupLog>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<ProfessionalCleanupLog>.Success(createdLog));

            var result = await controller.LogCleanup(companyId, logDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var dto = Assert.IsType<ProfessionalCleanupLogDto>(createdResult.Value);
            Assert.Equal(logDto.Notes, dto.Notes);
        }

        [Fact]
        public async Task GetAssignments_Success_ReturnsOk()
        {
            SetupAuthSuccess();

            var companyId = Guid.NewGuid();
            var company = new ProfessionalCompany { Id = companyId, Name = "Test Company", PartnerId = Guid.NewGuid(), IsActive = true };
            var adoptions = new List<SponsoredAdoption>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SponsorId = Guid.NewGuid(),
                    AdoptableAreaId = Guid.NewGuid(),
                    ProfessionalCompanyId = companyId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = "Active",
                },
            };

            companyManager
                .Setup(m => m.GetAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
            adoptionManager
                .Setup(m => m.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoptions);

            var result = await controller.GetAssignments(companyId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<SponsoredAdoptionDto>>(okResult.Value);
            Assert.Single(dtos);
        }
    }
}
