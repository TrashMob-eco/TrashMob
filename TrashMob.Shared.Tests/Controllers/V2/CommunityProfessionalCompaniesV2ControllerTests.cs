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
    using Xunit;

    public class CommunityProfessionalCompaniesV2ControllerTests
    {
        private readonly Mock<IProfessionalCompanyManager> companyManager = new();
        private readonly Mock<IProfessionalCompanyUserManager> companyUserManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<CommunityProfessionalCompaniesV2Controller>> logger = new();
        private readonly CommunityProfessionalCompaniesV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunityProfessionalCompaniesV2ControllerTests()
        {
            controller = new CommunityProfessionalCompaniesV2Controller(
                companyManager.Object, companyUserManager.Object, partnerManager.Object,
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
        public async Task GetCompanies_Authorized_ReturnsOkWithList()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var companies = new List<ProfessionalCompany>
            {
                new() { Id = Guid.NewGuid(), Name = "Company A", PartnerId = partnerId, IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "Company B", PartnerId = partnerId, IsActive = true },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            companyManager
                .Setup(m => m.GetByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(companies);

            var result = await controller.GetCompanies(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProfessionalCompanyDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            Assert.Equal("Company A", dtos.First().Name);
        }

        [Fact]
        public async Task CreateCompany_Authorized_ReturnsCreated()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var companyDto = new ProfessionalCompanyDto
            {
                Name = "New Company",
                ContactEmail = "company@test.com",
                IsActive = true,
            };
            var createdCompany = new ProfessionalCompany
            {
                Id = Guid.NewGuid(),
                Name = "New Company",
                ContactEmail = "company@test.com",
                PartnerId = partnerId,
                IsActive = true,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            companyManager
                .Setup(m => m.AddAsync(It.IsAny<ProfessionalCompany>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCompany);

            var result = await controller.CreateCompany(partnerId, companyDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var dto = Assert.IsType<ProfessionalCompanyDto>(createdResult.Value);
            Assert.Equal("New Company", dto.Name);
        }

        [Fact]
        public async Task AssignUser_Authorized_ReturnsCreated()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var assignedUserId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var company = new ProfessionalCompany { Id = companyId, Name = "Test Company", PartnerId = partnerId };
            var companyUserDto = new ProfessionalCompanyUserDto
            {
                ProfessionalCompanyId = companyId,
                UserId = assignedUserId,
            };
            var createdUser = new ProfessionalCompanyUser
            {
                ProfessionalCompanyId = companyId,
                UserId = assignedUserId,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            companyManager
                .Setup(m => m.GetAsync(companyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);
            companyUserManager
                .Setup(m => m.AddAsync(It.IsAny<ProfessionalCompanyUser>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            var result = await controller.AssignUser(partnerId, companyId, companyUserDto, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
            var dto = Assert.IsType<ProfessionalCompanyUserDto>(objectResult.Value);
            Assert.Equal(assignedUserId, dto.UserId);
        }
    }
}
