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
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class UserManagerTests
    {
        private readonly Mock<IKeyedRepository<User>> _userRepository;
        private readonly Mock<IUserDeletionService> _userDeletionService;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly UserManager _sut;

        public UserManagerTests()
        {
            _userRepository = new Mock<IKeyedRepository<User>>();
            _userDeletionService = new Mock<IUserDeletionService>();
            _emailManager = new Mock<IEmailManager>();

            // Default email manager setup
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new UserManager(
                _userRepository.Object,
                _userDeletionService.Object,
                _emailManager.Object);
        }

        #region GetUserByNameIdentifierAsync

        [Fact]
        public async Task GetUserByNameIdentifierAsync_ReturnsUserWhenFound()
        {
            // Arrange
            var nameIdentifier = "test-name-identifier";
            var user = new UserBuilder().Build();
            user.NameIdentifier = nameIdentifier;

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.GetUserByNameIdentifierAsync(nameIdentifier);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameIdentifier, result.NameIdentifier);
        }

        [Fact]
        public async Task GetUserByNameIdentifierAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.GetUserByNameIdentifierAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserByObjectIdAsync

        [Fact]
        public async Task GetUserByObjectIdAsync_ReturnsUserWhenFound()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            var user = new UserBuilder().Build();
            user.ObjectId = objectId;

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.GetUserByObjectIdAsync(objectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(objectId, result.ObjectId);
        }

        [Fact]
        public async Task GetUserByObjectIdAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.GetUserByObjectIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserByUserNameAsync

        [Fact]
        public async Task GetUserByUserNameAsync_ReturnsUserWhenFound()
        {
            // Arrange
            var userName = "testuser";
            var user = new UserBuilder().WithUserName(userName).Build();

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.GetUserByUserNameAsync(userName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userName, result.UserName);
        }

        [Fact]
        public async Task GetUserByUserNameAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.GetUserByUserNameAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserByEmailAsync

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsUserWhenFound()
        {
            // Arrange
            var email = "test@example.com";
            var user = new UserBuilder().WithEmail(email).Build();

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.GetUserByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserByInternalIdAsync

        [Fact]
        public async Task GetUserByInternalIdAsync_ReturnsUserWhenFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).Build();

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.GetUserByInternalIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUserByInternalIdAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.GetUserByInternalIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_PreservesIsSiteAdminFlag()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new UserBuilder().WithId(userId).AsSiteAdmin().Build();
            var updatedUser = new UserBuilder().WithId(userId).Build();
            updatedUser.IsSiteAdmin = false; // User trying to change admin flag

            _userRepository.SetupGetWithFilter(new List<User> { existingUser });
            _userRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _sut.UpdateAsync(updatedUser);

            // Assert
            Assert.True(result.IsSiteAdmin); // Flag should be preserved
        }

        [Fact]
        public async Task UpdateAsync_DoesNotGrantSiteAdminFromUserInput()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new UserBuilder().WithId(userId).Build();
            existingUser.IsSiteAdmin = false;
            var updatedUser = new UserBuilder().WithId(userId).Build();
            updatedUser.IsSiteAdmin = true; // User trying to grant themselves admin

            _userRepository.SetupGetWithFilter(new List<User> { existingUser });
            _userRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _sut.UpdateAsync(updatedUser);

            // Assert
            Assert.False(result.IsSiteAdmin); // Flag should remain false
        }

        #endregion

        #region AddAsync

        [Fact]
        public async Task AddAsync_SetsRequiredFieldsOnNewUser()
        {
            // Arrange
            var newUser = new UserBuilder().Build();
            newUser.Id = Guid.Empty; // Simulating unset ID

            User capturedUser = null;
            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _sut.AddAsync(newUser, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotEqual(Guid.Empty, capturedUser.Id);
            Assert.Equal(DateTimeOffset.MinValue, capturedUser.DateAgreedToTrashMobWaiver);
            Assert.Equal(string.Empty, capturedUser.TrashMobWaiverVersion);
            Assert.False(capturedUser.IsSiteAdmin);
        }

        [Fact]
        public async Task AddAsync_SendsWelcomeEmail()
        {
            // Arrange
            var newUser = new UserBuilder()
                .WithUserName("newuser")
                .WithEmail("newuser@example.com")
                .Build();

            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act
            await _sut.AddAsync(newUser, CancellationToken.None);

            // Assert - verify welcome email sent (second call)
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "Welcome to TrashMob.eco!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(addr => addr.Email == "newuser@example.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_NotifiesAdminsOfNewUser()
        {
            // Arrange
            var newUser = new UserBuilder()
                .WithEmail("newuser@example.com")
                .Build();

            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act
            await _sut.AddAsync(newUser, CancellationToken.None);

            // Assert - verify admin notification sent (first call)
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "New User Alert",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(addr => addr.Email == Constants.TrashMobEmailAddress)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UserExistsAsync

        [Fact]
        public async Task UserExistsAsync_ById_ReturnsTrueWhenExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).Build();

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.UserExistsAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_ById_ReturnsFalseWhenNotExists()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.UserExistsAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UserExistsAsync_ByNameIdentifier_ReturnsUserWhenExists()
        {
            // Arrange
            var nameIdentifier = "test-name-id";
            var user = new UserBuilder().Build();
            user.NameIdentifier = nameIdentifier;

            _userRepository.SetupGetWithFilter(new List<User> { user });

            // Act
            var result = await _sut.UserExistsAsync(nameIdentifier);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameIdentifier, result.NameIdentifier);
        }

        [Fact]
        public async Task UserExistsAsync_ByNameIdentifier_ReturnsNullWhenNotExists()
        {
            // Arrange
            _userRepository.SetupGetWithFilter(new List<User>());

            // Act
            var result = await _sut.UserExistsAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}
