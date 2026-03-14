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
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class ContactsV2ControllerTests
    {
        private readonly Mock<IContactManager> contactManager = new();
        private readonly Mock<IBaseManager<ContactContactTag>> contactContactTagManager = new();
        private readonly Mock<ILogger<ContactsV2Controller>> logger = new();
        private readonly ContactsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public ContactsV2ControllerTests()
        {
            controller = new ContactsV2Controller(
                contactManager.Object, contactContactTagManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var contacts = new List<Contact>
            {
                new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User", Email = "test@test.com" },
                new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Email = "jane@test.com" },
            };

            contactManager
                .Setup(m => m.SearchAsync("Test", It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            var result = await controller.GetAll(search: "Test", cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ContactDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            contactManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact)null);

            var result = await controller.GetById(id, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var contact = new Contact { Id = id, FirstName = "Test", LastName = "User", Email = "test@test.com" };

            contactManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            var result = await controller.GetById(id, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ContactDto>(okResult.Value);
            Assert.Equal("Test", dto.FirstName);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var contact = new Contact { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User", Email = "test@test.com" };

            contactManager
                .Setup(m => m.AddAsync(It.IsAny<Contact>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            var dto = new ContactDto { FirstName = "Test", LastName = "User", Email = "test@test.com" };

            var result = await controller.Create(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<ContactDto>(createdResult.Value);
            Assert.Equal("Test", resultDto.FirstName);
        }
    }
}
