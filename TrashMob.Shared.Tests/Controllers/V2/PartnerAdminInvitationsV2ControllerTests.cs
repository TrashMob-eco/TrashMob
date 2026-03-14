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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class PartnerAdminInvitationsV2ControllerTests
    {
        private readonly Mock<IKeyedManager<User>> userManager = new();
        private readonly Mock<IPartnerAdminInvitationManager> partnerAdminInvitationManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerAdminInvitationsV2Controller>> logger = new();
        private readonly PartnerAdminInvitationsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerAdminInvitationsV2ControllerTests()
        {
            controller = new PartnerAdminInvitationsV2Controller(
                userManager.Object,
                partnerAdminInvitationManager.Object,
                partnerManager.Object,
                authorizationService.Object,
                logger.Object);

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
        public async Task GetByPartner_Authorized_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var invitations = new List<PartnerAdminInvitation>
            {
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Email = "user1@test.com", InvitationStatusId = 1 },
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Email = "user2@test.com", InvitationStatusId = 1 },
            };

            partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            partnerAdminInvitationManager.Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invitations);

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerAdminInvitationDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetByPartner_Unauthorized_ReturnsForbid()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };

            partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_ReturnsCreated()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var dto = new PartnerAdminInvitationDto
            {
                PartnerId = partnerId,
                Email = "newadmin@test.com",
                InvitationStatusId = 1,
            };
            var user = new User { Id = Guid.NewGuid(), Email = "newadmin@test.com" };
            var created = new PartnerAdminInvitation
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Email = "newadmin@test.com",
                InvitationStatusId = 1,
            };

            partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            userManager.Setup(m => m.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<User>)new List<User> { user });
            partnerAdminInvitationManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerAdminInvitation>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await controller.Add(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PartnerAdminInvitationDto>(createdResult.Value);
            Assert.Equal("newadmin@test.com", resultDto.Email);
        }

        [Fact]
        public async Task Accept_ReturnsOk()
        {
            var invitationId = Guid.NewGuid();

            partnerAdminInvitationManager
                .Setup(m => m.AcceptInvitationAsync(invitationId, testUserId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.Accept(invitationId, CancellationToken.None);

            Assert.IsType<OkResult>(result);
            partnerAdminInvitationManager.Verify(
                m => m.AcceptInvitationAsync(invitationId, testUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Decline_ReturnsOk()
        {
            var invitationId = Guid.NewGuid();

            partnerAdminInvitationManager
                .Setup(m => m.DeclineInvitationAsync(invitationId, testUserId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.Decline(invitationId, CancellationToken.None);

            Assert.IsType<OkResult>(result);
            partnerAdminInvitationManager.Verify(
                m => m.DeclineInvitationAsync(invitationId, testUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            var invitationId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            partnerAdminInvitationManager
                .Setup(m => m.GetPartnerForInvitation(invitationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            partnerAdminInvitationManager
                .Setup(m => m.DeleteAsync(invitationId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            var result = await controller.Delete(invitationId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            partnerAdminInvitationManager.Verify(
                m => m.DeleteAsync(invitationId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Unauthorized_ReturnsForbid()
        {
            var invitationId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            partnerAdminInvitationManager
                .Setup(m => m.GetPartnerForInvitation(invitationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.Delete(invitationId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetByUser_ReturnsOkWithList()
        {
            var userId = Guid.NewGuid();
            var invitations = new List<DisplayPartnerAdminInvitation>
            {
                new() { Id = Guid.NewGuid(), PartnerName = "Partner A" },
            };

            partnerAdminInvitationManager
                .Setup(m => m.GetInvitationsForUser(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invitations);

            var result = await controller.GetByUser(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var results = Assert.IsAssignableFrom<IEnumerable<DisplayPartnerAdminInvitation>>(okResult.Value);
            Assert.Single(results);
        }
    }
}
