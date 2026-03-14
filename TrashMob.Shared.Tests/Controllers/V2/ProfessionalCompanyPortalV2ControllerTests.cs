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
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class ProfessionalCompanyPortalV2ControllerTests
    {
        private readonly Mock<IProfessionalCompanyUserManager> companyUserManager = new();
        private readonly Mock<ILogger<ProfessionalCompanyPortalV2Controller>> logger = new();
        private readonly ProfessionalCompanyPortalV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public ProfessionalCompanyPortalV2ControllerTests()
        {
            controller = new ProfessionalCompanyPortalV2Controller(
                companyUserManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetMyCompanies_ReturnsOk()
        {
            var companies = new List<ProfessionalCompany>
            {
                new() { Id = Guid.NewGuid(), Name = "Company A", PartnerId = Guid.NewGuid(), IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "Company B", PartnerId = Guid.NewGuid(), IsActive = true },
            };

            companyUserManager
                .Setup(m => m.GetCompaniesByUserIdAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(companies);

            var result = await controller.GetMyCompanies(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProfessionalCompanyDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetMyCompanies_Empty_ReturnsOkWithEmptyList()
        {
            companyUserManager
                .Setup(m => m.GetCompaniesByUserIdAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProfessionalCompany>());

            var result = await controller.GetMyCompanies(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProfessionalCompanyDto>>(okResult.Value);
            Assert.Empty(dtos);
        }
    }
}
