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

    public class PartnerContactsV2ControllerTests
    {
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IPartnerContactManager> partnerContactManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerContactsV2Controller>> logger = new();
        private readonly PartnerContactsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerContactsV2ControllerTests()
        {
            controller = new PartnerContactsV2Controller(
                partnerManager.Object, partnerContactManager.Object,
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
        public async Task GetByPartner_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var contacts = new List<PartnerContact>
            {
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Name = "Alice", Email = "alice@test.com" },
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Name = "Bob", Email = "bob@test.com" },
            };

            partnerContactManager
                .Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerContactDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var contactId = Guid.NewGuid();
            var contact = new PartnerContact
            {
                Id = contactId,
                PartnerId = Guid.NewGuid(),
                Name = "Alice",
                Email = "alice@test.com",
                Phone = "555-1234",
            };

            partnerContactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            var result = await controller.Get(contactId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerContactDto>(okResult.Value);
            Assert.Equal(contactId, dto.Id);
            Assert.Equal("Alice", dto.Name);
        }

        [Fact]
        public async Task Get_NotFound_Returns404()
        {
            var contactId = Guid.NewGuid();

            partnerContactManager
                .Setup(m => m.GetAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerContact)null);

            var result = await controller.Get(contactId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var dto = new PartnerContactDto
            {
                PartnerId = partnerId,
                Name = "New Contact",
                Email = "new@test.com",
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            partnerContactManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerContact>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PartnerContact
                {
                    Id = Guid.NewGuid(),
                    PartnerId = partnerId,
                    Name = "New Contact",
                    Email = "new@test.com",
                });

            var result = await controller.Add(dto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultDto = Assert.IsType<PartnerContactDto>(okResult.Value);
            Assert.Equal("New Contact", resultDto.Name);
        }

        [Fact]
        public async Task Add_PartnerNotFound_Returns404()
        {
            var dto = new PartnerContactDto
            {
                PartnerId = Guid.NewGuid(),
                Name = "Contact",
            };

            partnerManager
                .Setup(m => m.GetAsync(dto.PartnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.Add(dto, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            SetupAuthSuccess();

            var contactId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            partnerContactManager
                .Setup(m => m.GetPartnerForContact(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            partnerContactManager
                .Setup(m => m.DeleteAsync(contactId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            var result = await controller.Delete(contactId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_PartnerNotFound_Returns404()
        {
            var contactId = Guid.NewGuid();

            partnerContactManager
                .Setup(m => m.GetPartnerForContact(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.Delete(contactId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
