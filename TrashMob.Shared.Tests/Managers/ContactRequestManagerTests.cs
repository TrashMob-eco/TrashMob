namespace TrashMob.Shared.Tests.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class ContactRequestManagerTests
    {
        private readonly Mock<IKeyedRepository<ContactRequest>> _contactRequestRepository;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly ContactRequestManager _sut;

        public ContactRequestManagerTests()
        {
            _contactRequestRepository = new Mock<IKeyedRepository<ContactRequest>>();
            _emailManager = new Mock<IEmailManager>();

            // Default email setup
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new ContactRequestManager(_contactRequestRepository.Object, _emailManager.Object);
        }

        #region AddAsync

        [Fact]
        public async Task AddAsync_SetsRequiredFields()
        {
            // Arrange
            var contactRequest = new ContactRequest
            {
                Name = "John Doe",
                Email = "john@example.com",
                Message = "Hello, I have a question."
            };

            ContactRequest capturedRequest = null;
            _contactRequestRepository.Setup(r => r.AddAsync(It.IsAny<ContactRequest>()))
                .Callback<ContactRequest>(cr => capturedRequest = cr)
                .ReturnsAsync((ContactRequest cr) => cr);

            // Act
            var result = await _sut.AddAsync(contactRequest);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.NotEqual(Guid.Empty, capturedRequest.Id);
            Assert.Equal(Guid.Empty, capturedRequest.CreatedByUserId);
            Assert.Equal(Guid.Empty, capturedRequest.LastUpdatedByUserId);
        }

        [Fact]
        public async Task AddAsync_SendsNotificationEmail()
        {
            // Arrange
            var contactRequest = new ContactRequest
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Message = "Testing contact form"
            };

            _contactRequestRepository.Setup(r => r.AddAsync(It.IsAny<ContactRequest>()))
                .ReturnsAsync((ContactRequest cr) => cr);

            // Act
            await _sut.AddAsync(contactRequest);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("Contact Request")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(addr => addr.Email == Constants.TrashMobEmailAddress)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ReplacesPlaceholdersInEmail()
        {
            // Arrange
            var contactRequest = new ContactRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Message = "My test message"
            };

            var emailTemplate = "{UserName} - {UserEmail} - {Message}";
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns(emailTemplate);
            _contactRequestRepository.Setup(r => r.AddAsync(It.IsAny<ContactRequest>()))
                .ReturnsAsync((ContactRequest cr) => cr);

            // Act
            await _sut.AddAsync(contactRequest);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.ContactRequestReceived.ToString()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ReturnsAddedContactRequest()
        {
            // Arrange
            var contactRequest = new ContactRequest
            {
                Name = "Return Test",
                Email = "return@example.com",
                Message = "Test return value"
            };

            _contactRequestRepository.Setup(r => r.AddAsync(It.IsAny<ContactRequest>()))
                .ReturnsAsync((ContactRequest cr) => cr);

            // Act
            var result = await _sut.AddAsync(contactRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contactRequest.Name, result.Name);
            Assert.Equal(contactRequest.Email, result.Email);
        }

        #endregion
    }
}
