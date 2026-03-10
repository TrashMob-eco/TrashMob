namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class DependentInvitationsV2ControllerTests
    {
        private readonly Mock<IDependentInvitationManager> invitationManager = new();
        private readonly Mock<IDependentManager> dependentManager = new();
        private readonly Mock<ILogger<DependentInvitationsV2Controller>> logger = new();
        private readonly DependentInvitationsV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public DependentInvitationsV2ControllerTests()
        {
            controller = new DependentInvitationsV2Controller(invitationManager.Object, dependentManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task GetInvitationsForDependent_ReturnsOk()
        {
            var dependentId = Guid.NewGuid();
            var invitations = new List<DependentInvitation>
            {
                new() { Id = Guid.NewGuid() },
            };

            invitationManager.Setup(m => m.GetByDependentIdAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invitations);

            var result = await controller.GetInvitationsForDependent(userId, dependentId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<DependentInvitation>>(okResult.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task GetInvitationsForDependent_ReturnsForbid_WhenNotParent()
        {
            var otherUserId = Guid.NewGuid();
            var dependentId = Guid.NewGuid();

            var result = await controller.GetInvitationsForDependent(otherUserId, dependentId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CreateInvitation_ReturnsCreated()
        {
            var dependentId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = userId };
            var request = new CreateDependentInvitationRequest { Email = "minor@example.com" };
            var invitation = new DependentInvitation { Id = Guid.NewGuid() };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);
            invitationManager.Setup(m => m.CreateInvitationAsync(dependentId, request.Email, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invitation);

            var result = await controller.CreateInvitation(userId, dependentId, request, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateInvitation_ReturnsForbid_WhenNotParent()
        {
            var otherUserId = Guid.NewGuid();
            var dependentId = Guid.NewGuid();
            var request = new CreateDependentInvitationRequest { Email = "minor@example.com" };

            var result = await controller.CreateInvitation(otherUserId, dependentId, request, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CancelInvitation_ReturnsOk()
        {
            var invitationId = Guid.NewGuid();

            invitationManager.Setup(m => m.CancelInvitationAsync(invitationId, userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.CancelInvitation(invitationId, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task CancelInvitation_ReturnsBadRequest_WhenInvalidOperation()
        {
            var invitationId = Guid.NewGuid();

            invitationManager.Setup(m => m.CancelInvitationAsync(invitationId, userId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Invitation already cancelled"));

            var result = await controller.CancelInvitation(invitationId, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invitation already cancelled", badRequest.Value);
        }

        [Fact]
        public async Task ResendInvitation_ReturnsOk()
        {
            var invitationId = Guid.NewGuid();
            var updatedInvitation = new DependentInvitation { Id = invitationId };

            invitationManager.Setup(m => m.ResendInvitationAsync(invitationId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedInvitation);

            var result = await controller.ResendInvitation(invitationId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<DependentInvitation>(okResult.Value);
            Assert.Equal(invitationId, returned.Id);
        }
    }
}
