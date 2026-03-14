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

    public class PartnerLocationContactsV2ControllerTests
    {
        private readonly Mock<IPartnerLocationManager> partnerLocationManager = new();
        private readonly Mock<IPartnerLocationContactManager> partnerLocationContactManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerLocationContactsV2Controller>> logger = new();
        private readonly PartnerLocationContactsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerLocationContactsV2ControllerTests()
        {
            controller = new PartnerLocationContactsV2Controller(
                partnerLocationManager.Object,
                partnerLocationContactManager.Object,
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

        [Fact]
        public async Task GetByLocation_ReturnsOkWithList()
        {
            var partnerLocationId = Guid.NewGuid();
            var contacts = new List<PartnerLocationContact>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    PartnerLocationId = partnerLocationId,
                    Name = "John Doe",
                    Email = "john@example.com",
                    Phone = "555-1234",
                    IsActive = true,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    PartnerLocationId = partnerLocationId,
                    Name = "Jane Smith",
                    Email = "jane@example.com",
                    IsActive = true,
                },
            };

            partnerLocationContactManager
                .Setup(m => m.GetByParentIdAsync(partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            var result = await controller.GetByLocation(partnerLocationId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerLocationContactDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            Assert.Equal("John Doe", dtos.First().Name);
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var contactId = Guid.NewGuid();
            var contact = new PartnerLocationContact
            {
                Id = contactId,
                PartnerLocationId = Guid.NewGuid(),
                Name = "Contact Person",
                Email = "contact@example.com",
                IsActive = true,
            };

            partnerLocationContactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            var result = await controller.Get(contactId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerLocationContactDto>(okResult.Value);
            Assert.Equal(contactId, dto.Id);
            Assert.Equal("Contact Person", dto.Name);
        }

        [Fact]
        public async Task Get_NotFound_ReturnsNotFound()
        {
            var contactId = Guid.NewGuid();

            partnerLocationContactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerLocationContact)null);

            var result = await controller.Get(contactId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_ReturnsCreated()
        {
            var partnerLocationId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };
            var dto = new PartnerLocationContactDto
            {
                PartnerLocationId = partnerLocationId,
                Name = "New Contact",
                Email = "new@example.com",
                Phone = "555-9999",
                IsActive = true,
            };

            var createdEntity = new PartnerLocationContact
            {
                Id = Guid.NewGuid(),
                PartnerLocationId = partnerLocationId,
                Name = "New Contact",
                Email = "new@example.com",
                Phone = "555-9999",
                IsActive = true,
            };

            partnerLocationManager
                .Setup(m => m.GetPartnerForLocationAsync(partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            partnerLocationContactManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerLocationContact>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdEntity);

            var result = await controller.Add(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PartnerLocationContactDto>(createdResult.Value);
            Assert.Equal("New Contact", resultDto.Name);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            var contactId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            partnerLocationContactManager
                .Setup(m => m.GetPartnerForLocationContact(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            partnerLocationContactManager
                .Setup(m => m.DeleteAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(contactId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_PartnerNotFound_ReturnsNotFound()
        {
            var contactId = Guid.NewGuid();

            partnerLocationContactManager
                .Setup(m => m.GetPartnerForLocationContact(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.Delete(contactId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
