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
    using TrashMob.Shared.Managers.Prospects;
    using Xunit;

    public class ProspectContactsV2ControllerTests
    {
        private readonly Mock<ICommunityProspectManager> prospectManager = new();
        private readonly Mock<IProspectContactManager> contactManager = new();
        private readonly Mock<ILogger<ProspectContactsV2Controller>> logger = new();
        private readonly ProspectContactsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();
        private readonly Guid prospectId = Guid.NewGuid();

        public ProspectContactsV2ControllerTests()
        {
            controller = new ProspectContactsV2Controller(
                prospectManager.Object,
                contactManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_WhenProspectMissing_Returns404()
        {
            prospectManager
                .Setup(m => m.GetAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CommunityProspect)null);

            var result = await controller.GetAll(prospectId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ReturnsContactsForProspect()
        {
            prospectManager
                .Setup(m => m.GetAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommunityProspect { Id = prospectId, Name = "Test City" });
            contactManager
                .Setup(m => m.GetByProspectAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new ProspectContact { Id = Guid.NewGuid(), ProspectId = prospectId, Name = "Alice", IsPrimary = true },
                    new ProspectContact { Id = Guid.NewGuid(), ProspectId = prospectId, Name = "Bob" },
                });

            var result = await controller.GetAll(prospectId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<ProspectContactDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
        }

        [Fact]
        public async Task Get_WhenContactBelongsToDifferentProspect_Returns404()
        {
            var contactId = Guid.NewGuid();
            contactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact
                {
                    Id = contactId,
                    ProspectId = Guid.NewGuid(),  // Different prospect
                    Name = "Alice",
                });

            var result = await controller.Get(prospectId, contactId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenNameMissing_Returns400()
        {
            var result = await controller.Create(prospectId, new ProspectContactDto { Name = string.Empty }, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task Create_WhenProspectMissing_Returns404()
        {
            prospectManager
                .Setup(m => m.GetAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CommunityProspect)null);

            var result = await controller.Create(prospectId, new ProspectContactDto { Name = "Alice" }, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_StampsProspectIdAndAdds()
        {
            prospectManager
                .Setup(m => m.GetAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommunityProspect { Id = prospectId });
            contactManager
                .Setup(m => m.AddAsync(It.IsAny<ProspectContact>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProspectContact c, Guid _, CancellationToken _) =>
                {
                    c.Id = Guid.NewGuid();
                    return c;
                });

            var result = await controller.Create(prospectId, new ProspectContactDto { Name = "Alice", Email = "a@x.com" }, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var dto = Assert.IsType<ProspectContactDto>(createdResult.Value);
            Assert.Equal("Alice", dto.Name);
            contactManager.Verify(m => m.AddAsync(It.Is<ProspectContact>(c => c.ProspectId == prospectId && c.Name == "Alice"),
                testUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_WhenIsPrimaryTrue_CallsSetPrimary()
        {
            prospectManager
                .Setup(m => m.GetAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommunityProspect { Id = prospectId });
            var created = new ProspectContact { Id = Guid.NewGuid(), ProspectId = prospectId, Name = "Alice" };
            contactManager
                .Setup(m => m.AddAsync(It.IsAny<ProspectContact>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);
            contactManager
                .Setup(m => m.SetPrimaryAsync(created.Id, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact { Id = created.Id, ProspectId = prospectId, Name = "Alice", IsPrimary = true });

            await controller.Create(prospectId, new ProspectContactDto { Name = "Alice", IsPrimary = true }, CancellationToken.None);

            contactManager.Verify(m => m.SetPrimaryAsync(created.Id, testUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenContactHasReferences_Returns409()
        {
            var contactId = Guid.NewGuid();
            contactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact { Id = contactId, ProspectId = prospectId, Name = "Alice" });
            contactManager
                .Setup(m => m.HasReferencesAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.Delete(prospectId, contactId, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, objectResult.StatusCode);
            contactManager.Verify(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenNoReferences_Returns204()
        {
            var contactId = Guid.NewGuid();
            contactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact { Id = contactId, ProspectId = prospectId, Name = "Alice" });
            contactManager
                .Setup(m => m.HasReferencesAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.Delete(prospectId, contactId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            contactManager.Verify(m => m.DeleteAsync(contactId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetPrimary_DelegatesToManager()
        {
            var contactId = Guid.NewGuid();
            contactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact { Id = contactId, ProspectId = prospectId, Name = "Alice" });
            contactManager
                .Setup(m => m.SetPrimaryAsync(contactId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact { Id = contactId, ProspectId = prospectId, Name = "Alice", IsPrimary = true });

            var result = await controller.SetPrimary(prospectId, contactId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ProspectContactDto>(okResult.Value);
            Assert.True(dto.IsPrimary);
        }

        [Fact]
        public async Task UpdateStatus_DelegatesToManager()
        {
            var contactId = Guid.NewGuid();
            contactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact { Id = contactId, ProspectId = prospectId, Name = "Alice" });
            contactManager
                .Setup(m => m.UpdateStatusAsync(contactId, (int)ProspectContactStatus.WrongPerson, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProspectContact
                {
                    Id = contactId,
                    ProspectId = prospectId,
                    Name = "Alice",
                    ContactStatus = (int)ProspectContactStatus.WrongPerson,
                });

            var result = await controller.UpdateStatus(
                prospectId,
                contactId,
                new UpdateProspectContactStatusRequest { Status = (int)ProspectContactStatus.WrongPerson },
                CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ProspectContactDto>(okResult.Value);
            Assert.Equal((int)ProspectContactStatus.WrongPerson, dto.ContactStatus);
        }
    }
}
