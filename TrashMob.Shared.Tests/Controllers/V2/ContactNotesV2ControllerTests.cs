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
    using Xunit;

    public class ContactNotesV2ControllerTests
    {
        private readonly Mock<IContactNoteManager> contactNoteManager = new();
        private readonly Mock<ILogger<ContactNotesV2Controller>> logger = new();
        private readonly ContactNotesV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public ContactNotesV2ControllerTests()
        {
            controller = new ContactNotesV2Controller(contactNoteManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetByContactId_ReturnsOk()
        {
            var contactId = Guid.NewGuid();
            var notes = new List<ContactNote>
            {
                new() { Id = Guid.NewGuid(), ContactId = contactId, Subject = "Test", Body = "Note body" },
                new() { Id = Guid.NewGuid(), ContactId = contactId, Subject = "Follow-up", Body = "Second note" },
            };

            contactNoteManager
                .Setup(m => m.GetByContactIdAsync(contactId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notes);

            var result = await controller.GetByContactId(contactId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ContactNoteDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var note = new ContactNote { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), Subject = "Test", Body = "Note body" };

            contactNoteManager
                .Setup(m => m.AddAsync(It.IsAny<ContactNote>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(note);

            var dto = new ContactNoteDto { ContactId = note.ContactId, Subject = "Test", Body = "Note body" };

            var result = await controller.Create(dto, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var noteId = Guid.NewGuid();

            contactNoteManager
                .Setup(m => m.DeleteAsync(noteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(noteId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            contactNoteManager.Verify(
                m => m.DeleteAsync(noteId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
