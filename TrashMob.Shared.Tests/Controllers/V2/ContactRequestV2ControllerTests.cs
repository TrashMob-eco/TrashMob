namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class ContactRequestV2ControllerTests
    {
        private readonly Mock<IKeyedManager<ContactRequest>> contactRequestManager = new();
        private readonly Mock<ILogger<ContactRequestV2Controller>> logger = new();
        private readonly ContactRequestV2Controller controller;

        public ContactRequestV2ControllerTests()
        {
            controller = new ContactRequestV2Controller(contactRequestManager.Object, logger.Object);
        }

        [Fact]
        public async Task Add_Returns201Created()
        {
            var dto = new ContactRequestDto
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Message = "I want to help!",
            };

            contactRequestManager
                .Setup(m => m.AddAsync(It.IsAny<ContactRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactRequest());

            var result = await controller.Add(dto, CancellationToken.None);

            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, statusResult.StatusCode);

            contactRequestManager.Verify(
                m => m.AddAsync(
                    It.Is<ContactRequest>(e =>
                        e.Name == "Jane Doe" &&
                        e.Email == "jane@example.com" &&
                        e.Message == "I want to help!"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
