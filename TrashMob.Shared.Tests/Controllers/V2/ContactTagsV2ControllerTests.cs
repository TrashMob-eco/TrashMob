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

    public class ContactTagsV2ControllerTests
    {
        private readonly Mock<IKeyedManager<ContactTag>> contactTagManager = new();
        private readonly Mock<ILogger<ContactTagsV2Controller>> logger = new();
        private readonly ContactTagsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public ContactTagsV2ControllerTests()
        {
            controller = new ContactTagsV2Controller(contactTagManager.Object, logger.Object);

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
            var tags = new List<ContactTag>
            {
                new() { Id = Guid.NewGuid(), Name = "VIP", Color = "#FF0000" },
                new() { Id = Guid.NewGuid(), Name = "Donor", Color = "#00FF00" },
            };

            contactTagManager
                .Setup(m => m.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tags);

            var result = await controller.GetAll(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ContactTagDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var tag = new ContactTag { Id = Guid.NewGuid(), Name = "VIP", Color = "#FF0000" };

            contactTagManager
                .Setup(m => m.AddAsync(It.IsAny<ContactTag>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tag);

            var dto = new ContactTagDto { Name = "VIP", Color = "#FF0000" };

            var result = await controller.Create(dto, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var tagId = Guid.NewGuid();

            contactTagManager
                .Setup(m => m.DeleteAsync(tagId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(tagId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
