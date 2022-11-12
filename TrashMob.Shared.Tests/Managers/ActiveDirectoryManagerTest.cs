namespace TrashMob.Shared.Tests
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class ActiveDirectoryManagerTest  
    {
        public ActiveDirectoryManagerTest()
        {
        }

        [Fact]
        public async Task ValidateUserAsync_ValidUser_Succeeds()
        {
            var mockUserManager = new Mock<IUserManager>();
            mockUserManager.Setup(m => m.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(null));
            mockUserManager.Setup(m => m.GetUserByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(null));

            var activeDirectoryManager = new ActiveDirectoryManager(mockUserManager.Object);
            var activeDirectoryValidateUserRequest = new ActiveDirectoryValidateNewUserRequest
            {
                email = "joe@test.com",
                userName = "joe"
            };

            var result = await activeDirectoryManager.ValidateNewUserAsync(activeDirectoryValidateUserRequest);

            // Assert
            Assert.Equal("Continue", result.action);
            Assert.Equal("1.0.0", result.version);
        }

        [Fact]
        public async Task ValidateUserAsync_EmailExists_Fails()
        {
            var mockUserManager = new Mock<IUserManager>();
            var user = new User { Email = "joe@test.com", UserName = "joe" };
            mockUserManager.Setup(m => m.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(user));
            mockUserManager.Setup(m => m.GetUserByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(null));

            var activeDirectoryManager = new ActiveDirectoryManager(mockUserManager.Object);
            var activeDirectoryValidateUserRequest = new ActiveDirectoryValidateNewUserRequest
            {
                email = "joe@test.com",
                userName = "joe"
            };

            var result = await activeDirectoryManager.ValidateNewUserAsync(activeDirectoryValidateUserRequest);

            // Assert
            Assert.Equal("ValidationError", result.action);
            Assert.Equal("1.0.0", result.version);
        }

        [Fact]
        public async Task ValidateUserAsync_UserNameExists_Fails()
        {
            var mockUserManager = new Mock<IUserManager>();
            var user = new User { Email = "joe@test.com", UserName = "joe" };
            mockUserManager.Setup(m => m.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(null));
            mockUserManager.Setup(m => m.GetUserByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(user));

            var activeDirectoryManager = new ActiveDirectoryManager(mockUserManager.Object);
            var activeDirectoryValidateUserRequest = new ActiveDirectoryValidateNewUserRequest
            {
                email = "joe@test.com",
                userName = "joe"
            };

            var result = await activeDirectoryManager.ValidateNewUserAsync(activeDirectoryValidateUserRequest);

            // Assert
            Assert.Equal("ValidationError", result.action);
            Assert.Equal("1.0.0", result.version);
        }

        [Fact]
        public async Task CreateUserAsync_EmailExists_Fails()
        {
            var mockUserManager = new Mock<IUserManager>();
            var user = new User { Email = "joe@test.com", UserName = "joe" };
            mockUserManager.Setup(m => m.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(user));
            mockUserManager.Setup(m => m.GetUserByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(null));

            var activeDirectoryManager = new ActiveDirectoryManager(mockUserManager.Object);
            var activeDirectoryNewUserRequest = new ActiveDirectoryNewUserRequest
            {
                email = "joe@test.com",
                userName = "joe"
            };

            var result = await activeDirectoryManager.CreateUserAsync(activeDirectoryNewUserRequest);

            // Assert
            Assert.Equal("ValidationError", result.action);
            Assert.Equal("1.0.0", result.version);
        }

        [Fact]
        public async Task CreateUserAsync_UserNameExists_Fails()
        {
            var mockUserManager = new Mock<IUserManager>();
            var user = new User { Email = "joe@test.com", UserName = "joe" };
            mockUserManager.Setup(m => m.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(null));
            mockUserManager.Setup(m => m.GetUserByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<User>(user));

            var activeDirectoryManager = new ActiveDirectoryManager(mockUserManager.Object);
            var activeDirectoryNewUserRequest = new ActiveDirectoryNewUserRequest
            {
                email = "joe@test.com",
                userName = "joe"
            };

            var result = await activeDirectoryManager.CreateUserAsync(activeDirectoryNewUserRequest);

            // Assert
            Assert.Equal("ValidationError", result.action);
            Assert.Equal("1.0.0", result.version);
        }

        public static IEnumerable<object[]> enumValues()
        {
            foreach (var notificationType in Enum.GetValues(typeof(NotificationTypeEnum)))
            {
                yield return new object[] { notificationType.ToString() };
            }
        }
    }
}

