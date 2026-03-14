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

    public class CommunityInvitesV2ControllerTests
    {
        private readonly Mock<IEmailInviteManager> emailInviteManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<CommunityInvitesV2Controller>> logger = new();
        private readonly CommunityInvitesV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunityInvitesV2ControllerTests()
        {
            controller = new CommunityInvitesV2Controller(
                emailInviteManager.Object, partnerManager.Object,
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
        public async Task GetBatches_Authorized_ReturnsOkWithList()
        {
            SetupAuthSuccess();

            var communityId = Guid.NewGuid();
            var partner = new Partner { Id = communityId, Name = "Test Community" };
            var batches = new List<EmailInviteBatch>
            {
                new() { Id = Guid.NewGuid(), BatchType = "Community", CommunityId = communityId, TotalCount = 5, Status = "Complete" },
                new() { Id = Guid.NewGuid(), BatchType = "Community", CommunityId = communityId, TotalCount = 3, Status = "Pending" },
            };

            partnerManager
                .Setup(m => m.GetAsync(communityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            emailInviteManager
                .Setup(m => m.GetCommunityBatchesAsync(communityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batches);

            var result = await controller.GetBatches(communityId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<EmailInviteBatchDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
        }

        [Fact]
        public async Task GetBatch_Found_ReturnsOk()
        {
            SetupAuthSuccess();

            var communityId = Guid.NewGuid();
            var batchId = Guid.NewGuid();
            var partner = new Partner { Id = communityId, Name = "Test Community" };
            var batch = new EmailInviteBatch
            {
                Id = batchId,
                BatchType = "Community",
                CommunityId = communityId,
                TotalCount = 10,
                SentCount = 8,
                Status = "Complete",
            };

            partnerManager
                .Setup(m => m.GetAsync(communityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            emailInviteManager
                .Setup(m => m.GetBatchDetailsAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batch);

            var result = await controller.GetBatch(communityId, batchId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EmailInviteBatchDto>(okResult.Value);
            Assert.Equal(batchId, dto.Id);
            Assert.Equal(10, dto.TotalCount);
        }

        [Fact]
        public async Task CreateBatch_Authorized_ReturnsCreated()
        {
            SetupAuthSuccess();

            var communityId = Guid.NewGuid();
            var partner = new Partner { Id = communityId, Name = "Test Community" };
            var dto = new CreateEmailInviteBatchDto
            {
                Emails = new List<string> { "user1@test.com", "user2@test.com" },
            };
            var createdBatch = new EmailInviteBatch
            {
                Id = Guid.NewGuid(),
                BatchType = "Community",
                CommunityId = communityId,
                TotalCount = 2,
                Status = "Pending",
            };
            var processedBatch = new EmailInviteBatch
            {
                Id = createdBatch.Id,
                BatchType = "Community",
                CommunityId = communityId,
                TotalCount = 2,
                SentCount = 2,
                Status = "Complete",
            };

            partnerManager
                .Setup(m => m.GetAsync(communityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            emailInviteManager
                .Setup(m => m.CreateBatchAsync(
                    It.IsAny<IEnumerable<string>>(), testUserId, "Community", communityId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdBatch);
            emailInviteManager
                .Setup(m => m.ProcessBatchAsync(createdBatch.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(processedBatch);

            var result = await controller.CreateBatch(communityId, dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var batchDto = Assert.IsType<EmailInviteBatchDto>(createdResult.Value);
            Assert.Equal(2, batchDto.SentCount);
        }
    }
}
