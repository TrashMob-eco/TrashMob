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
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="RolesV2Controller"/> — the Phase 3 admin controller
    /// for role management. Covers the four endpoints (list roles, list members,
    /// grant, revoke) plus the notification wiring.
    /// </summary>
    public class RolesV2ControllerTests
    {
        private readonly Mock<IUserRoleService> userRoleService = new();
        private readonly Mock<IUserManager> userManager = new();
        private readonly Mock<IRoleGrantNotificationService> notificationService = new();
        private readonly Mock<ILogger<RolesV2Controller>> logger = new();
        private readonly RolesV2Controller controller;
        private readonly Guid actorUserId = Guid.NewGuid();

        public RolesV2ControllerTests()
        {
            controller = new RolesV2Controller(
                userRoleService.Object,
                userManager.Object,
                notificationService.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = actorUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, actorUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            userManager
                .Setup(m => m.GetAsync(actorUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = actorUserId, UserName = "actor", Email = "actor@test.com" });
        }

        [Fact]
        public async Task List_ReturnsRolesWithMemberCounts()
        {
            var siteAdmin = new Role { Id = 1, Name = RoleNames.SiteAdmin, DisplayOrder = 1, IsActive = true };
            var salesRep = new Role { Id = 2, Name = RoleNames.SalesRep, DisplayOrder = 2, IsActive = true };

            userRoleService.Setup(s => s.ListRolesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { siteAdmin, salesRep });
            userRoleService.Setup(s => s.GetUsersInRoleAsync(RoleNames.SiteAdmin, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new User { Id = Guid.NewGuid() }, new User { Id = Guid.NewGuid() } });
            userRoleService.Setup(s => s.GetUsersInRoleAsync(RoleNames.SalesRep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new User { Id = Guid.NewGuid() } });

            var result = await controller.List(CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<RoleDto>>(ok.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal(2, dtos.Single(d => d.Name == RoleNames.SiteAdmin).MemberCount);
            Assert.Equal(1, dtos.Single(d => d.Name == RoleNames.SalesRep).MemberCount);
        }

        [Fact]
        public async Task Members_UnknownRole_ReturnsNotFound()
        {
            userRoleService.Setup(s => s.ListRolesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new Role { Id = 1, Name = RoleNames.SiteAdmin } });

            var result = await controller.Members("NotARole", CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Members_KnownRole_ReturnsMemberList()
        {
            userRoleService.Setup(s => s.ListRolesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new Role { Id = 2, Name = RoleNames.SalesRep } });
            userRoleService.Setup(s => s.GetUsersInRoleAsync(RoleNames.SalesRep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new User { Id = Guid.NewGuid(), UserName = "rep1", MemberSince = DateTimeOffset.UtcNow.AddYears(-1) },
                });

            var result = await controller.Members(RoleNames.SalesRep, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<UserDto>>(ok.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetUserRoles_UnknownUser_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await controller.GetUserRoles(userId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUserRoles_KnownUser_ReturnsActiveGrants()
        {
            var userId = Guid.NewGuid();
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId });
            userRoleService.Setup(s => s.GetActiveGrantsForUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new UserRole
                    {
                        Id = Guid.NewGuid(), UserId = userId, RoleId = 2,
                        Role = new Role { Id = 2, Name = RoleNames.SalesRep },
                        GrantedByUserId = actorUserId, GrantedDate = DateTimeOffset.UtcNow.AddDays(-1),
                    },
                });

            var result = await controller.GetUserRoles(userId, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<UserRoleGrantDto>>(ok.Value);
            Assert.Single(dtos);
            Assert.Equal(RoleNames.SalesRep, dtos[0].RoleName);
        }

        [Fact]
        public async Task Grant_MissingRoleName_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            var result = await controller.Grant(userId, new GrantRoleRequest { RoleName = string.Empty }, CancellationToken.None);

            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        }

        [Fact]
        public async Task Grant_UnknownUser_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await controller.Grant(userId, new GrantRoleRequest { RoleName = RoleNames.SalesRep }, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Grant_HappyPath_ReturnsGrantAndSendsEmail()
        {
            var userId = Guid.NewGuid();
            var recipient = new User { Id = userId, UserName = "target", Email = "target@test.com" };
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(recipient);

            var grant = new UserRole
            {
                Id = Guid.NewGuid(), UserId = userId, RoleId = 2,
                Role = new Role { Id = 2, Name = RoleNames.SalesRep },
                GrantedByUserId = actorUserId, GrantedDate = DateTimeOffset.UtcNow,
            };
            userRoleService.Setup(s => s.GrantRoleAsync(userId, RoleNames.SalesRep, actorUserId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grant);

            var result = await controller.Grant(userId, new GrantRoleRequest { RoleName = RoleNames.SalesRep }, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserRoleGrantDto>(ok.Value);
            Assert.Equal(RoleNames.SalesRep, dto.RoleName);

            notificationService.Verify(
                n => n.SendRoleGrantedAsync(grant, recipient, It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Grant_InvalidRole_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "target", Email = "target@test.com" });

            userRoleService.Setup(s => s.GrantRoleAsync(userId, "NotARole", actorUserId, null, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Role 'NotARole' does not exist."));

            var result = await controller.Grant(userId, new GrantRoleRequest { RoleName = "NotARole" }, CancellationToken.None);

            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        }

        [Fact]
        public async Task Revoke_UnknownUser_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await controller.Revoke(userId, RoleNames.SalesRep, reason: null, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Revoke_NoActiveGrant_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "target", Email = "target@test.com" });
            userRoleService.Setup(s => s.RevokeRoleAsync(userId, RoleNames.SalesRep, actorUserId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserRole)null);

            var result = await controller.Revoke(userId, RoleNames.SalesRep, reason: null, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Revoke_HappyPath_ReturnsNoContentAndSendsEmail()
        {
            var userId = Guid.NewGuid();
            var recipient = new User { Id = userId, UserName = "target", Email = "target@test.com" };
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(recipient);

            var revoked = new UserRole
            {
                Id = Guid.NewGuid(), UserId = userId, RoleId = 2,
                Role = new Role { Id = 2, Name = RoleNames.SalesRep },
                GrantedByUserId = actorUserId, GrantedDate = DateTimeOffset.UtcNow.AddDays(-1),
                RevokedDate = DateTimeOffset.UtcNow, RevokedByUserId = actorUserId, RevokedReason = "contract ended",
            };
            userRoleService.Setup(s => s.RevokeRoleAsync(userId, RoleNames.SalesRep, actorUserId, "contract ended", It.IsAny<CancellationToken>()))
                .ReturnsAsync(revoked);

            var result = await controller.Revoke(userId, RoleNames.SalesRep, "contract ended", CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            notificationService.Verify(
                n => n.SendRoleRevokedAsync(revoked, recipient, It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
