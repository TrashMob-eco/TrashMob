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

    public class EmailInvitesV2ControllerTests
    {
        private readonly Mock<IEmailInviteManager> emailInviteManager = new();
        private readonly Mock<ILogger<EmailInvitesV2Controller>> logger = new();
        private readonly EmailInvitesV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public EmailInvitesV2ControllerTests()
        {
            controller = new EmailInvitesV2Controller(emailInviteManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task GetBatches_ReturnsOkWithList()
        {
            var batches = new List<EmailInviteBatch>
            {
                new() { Id = Guid.NewGuid(), SenderUserId = testUserId, BatchType = "User", TotalCount = 5, Status = "Complete" },
                new() { Id = Guid.NewGuid(), SenderUserId = testUserId, BatchType = "User", TotalCount = 3, Status = "Pending" },
            };

            emailInviteManager.Setup(m => m.GetUserBatchesAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batches);

            var result = await controller.GetBatches();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<EmailInviteBatchDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("User", dtos[0].BatchType);
        }

        [Fact]
        public async Task GetBatch_OwnBatch_ReturnsOk()
        {
            var batchId = Guid.NewGuid();
            emailInviteManager.Setup(m => m.GetBatchDetailsAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EmailInviteBatch
                {
                    Id = batchId,
                    SenderUserId = testUserId,
                    BatchType = "User",
                    TotalCount = 5,
                    Status = "Complete",
                });

            var result = await controller.GetBatch(batchId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EmailInviteBatchDto>(okResult.Value);
            Assert.Equal(batchId, dto.Id);
        }

        [Fact]
        public async Task GetBatch_OtherUserBatch_ReturnsForbid()
        {
            var batchId = Guid.NewGuid();
            emailInviteManager.Setup(m => m.GetBatchDetailsAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EmailInviteBatch
                {
                    Id = batchId,
                    SenderUserId = Guid.NewGuid(), // Different user
                    BatchType = "User",
                });

            var result = await controller.GetBatch(batchId);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetBatch_NotFound_Returns404()
        {
            emailInviteManager.Setup(m => m.GetBatchDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmailInviteBatch)null);

            var result = await controller.GetBatch(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetQuota_ReturnsOkWithQuota()
        {
            emailInviteManager.Setup(m => m.GetUserMonthlyInviteCountAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(15);

            var result = await controller.GetQuota();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var quota = Assert.IsType<EmailInviteQuotaDto>(okResult.Value);
            Assert.Equal(10, quota.MaxPerBatch);
            Assert.Equal(50, quota.MaxPerMonth);
            Assert.Equal(15, quota.UsedThisMonth);
            Assert.Equal(35, quota.RemainingThisMonth);
        }

        [Fact]
        public async Task CreateBatch_ValidInput_Returns201()
        {
            var batchId = Guid.NewGuid();
            var dto = new CreateEmailInviteBatchDto
            {
                Emails = new[] { "a@b.com", "c@d.com" },
            };

            emailInviteManager.Setup(m => m.GetUserMonthlyInviteCountAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            emailInviteManager.Setup(m => m.CreateBatchAsync(
                    It.IsAny<IEnumerable<string>>(), testUserId, "User", null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EmailInviteBatch { Id = batchId, SenderUserId = testUserId });

            emailInviteManager.Setup(m => m.ProcessBatchAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EmailInviteBatch
                {
                    Id = batchId,
                    SenderUserId = testUserId,
                    BatchType = "User",
                    TotalCount = 2,
                    SentCount = 2,
                    Status = "Complete",
                });

            var result = await controller.CreateBatch(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
            var resultDto = Assert.IsType<EmailInviteBatchDto>(objectResult.Value);
            Assert.Equal(2, resultDto.TotalCount);
        }

        [Fact]
        public async Task CreateBatch_EmptyEmails_ReturnsBadRequest()
        {
            var dto = new CreateEmailInviteBatchDto { Emails = Enumerable.Empty<string>() };

            var result = await controller.CreateBatch(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateBatch_QuotaExceeded_Returns429()
        {
            var dto = new CreateEmailInviteBatchDto
            {
                Emails = new[] { "a@b.com" },
            };

            emailInviteManager.Setup(m => m.GetUserMonthlyInviteCountAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(50); // At limit

            var result = await controller.CreateBatch(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(429, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateBatch_TooManyEmails_ReturnsBadRequest()
        {
            var dto = new CreateEmailInviteBatchDto
            {
                Emails = Enumerable.Range(1, 11).Select(i => $"user{i}@example.com"),
            };

            var result = await controller.CreateBatch(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }
    }
}
