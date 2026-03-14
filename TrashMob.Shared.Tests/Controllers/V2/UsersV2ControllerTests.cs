namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class UsersV2ControllerTests
    {
        private readonly Mock<IUserManager> userManager = new();
        private readonly Mock<IEventAttendeeMetricsManager> metricsManager = new();
        private readonly Mock<IImageManager> imageManager = new();
        private readonly Mock<IUserDataExportManager> exportManager = new();
        private readonly Mock<ILogger<UsersV2Controller>> logger = new();
        private readonly UsersV2Controller controller;

        public UsersV2ControllerTests()
        {
            controller = new UsersV2Controller(userManager.Object, metricsManager.Object, imageManager.Object, exportManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetUsers_ReturnsOkWithPagedResponse()
        {
            var users = new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserName = "alice",
                    City = "Seattle",
                    Region = "WA",
                    Country = "US",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserName = "bob",
                    City = "Portland",
                    Region = "OR",
                    Country = "US",
                },
            };

            var queryable = new TestAsyncEnumerable<User>(users);
            var filter = new UserQueryParameters { Page = 1, PageSize = 25 };

            userManager
                .Setup(m => m.GetFilteredUsersQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetUsers(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<UserDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal(2, response.Pagination.TotalCount);
            Assert.Equal("alice", response.Items[0].UserName);
            Assert.Equal("bob", response.Items[1].UserName);
        }

        [Fact]
        public async Task GetUsers_ReturnsEmptyPagedResponse_WhenNoUsers()
        {
            var queryable = new TestAsyncEnumerable<User>(Enumerable.Empty<User>());
            var filter = new UserQueryParameters { Page = 1, PageSize = 25 };

            userManager
                .Setup(m => m.GetFilteredUsersQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetUsers(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<UserDto>>(okResult.Value);
            Assert.Empty(response.Items);
            Assert.Equal(0, response.Pagination.TotalCount);
        }

        [Fact]
        public async Task GetUsers_AppliesPagination()
        {
            var users = Enumerable.Range(1, 5).Select(i => new User
            {
                Id = Guid.NewGuid(),
                UserName = $"user{i}",
            }).ToList();

            var queryable = new TestAsyncEnumerable<User>(users);
            var filter = new UserQueryParameters { Page = 2, PageSize = 2 };

            userManager
                .Setup(m => m.GetFilteredUsersQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetUsers(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<UserDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal(5, response.Pagination.TotalCount);
            Assert.Equal(2, response.Pagination.Page);
            Assert.True(response.Pagination.HasPrevious);
            Assert.True(response.Pagination.HasNext);
        }

        [Fact]
        public async Task GetUser_ReturnsOkWithUserDto()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                GivenName = "Test",
                Surname = "User",
                PrefersMetric = false,
                MemberSince = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            };

            userManager
                .Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await controller.GetUser(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, dto.Id);
            Assert.Equal("testuser", dto.UserName);
            Assert.Equal("Seattle", dto.City);
            Assert.Equal("Test", dto.GivenName);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            userManager
                .Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await controller.GetUser(userId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddUser_ReturnsCreated()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();
            var userDto = new UserWriteDto { UserName = "newuser" };
            var created = new User { Id = userId, UserName = "newuser" };

            userManager
                .Setup(m => m.AddAsync(It.IsAny<User>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await controller.AddUser(userDto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetUserByEmail_ReturnsOk_WhenFound()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            var user = new User { Id = Guid.NewGuid(), UserName = "emailuser", Email = "test@example.com" };

            userManager
                .Setup(m => m.GetUserByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await controller.GetUserByEmail("test@example.com", CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UserDto>(okResult.Value);
        }

        [Fact]
        public async Task GetUserByEmail_ReturnsNotFound_WhenNotFound()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();

            userManager
                .Setup(m => m.GetUserByEmailAsync("missing@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await controller.GetUserByEmail("missing@example.com", CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUserByObjectId_ReturnsOk_WhenFound()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            var objectId = Guid.NewGuid();
            var user = new User { Id = Guid.NewGuid(), UserName = "oiduser" };

            userManager
                .Setup(m => m.GetUserByObjectIdAsync(objectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await controller.GetUserByObjectId(objectId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UserDto>(okResult.Value);
        }

        [Fact]
        public async Task GetUserImpact_ReturnsNotFound_WhenUserNotFound()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            userManager
                .Setup(m => m.GetUserByInternalIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await controller.GetUserImpact(userId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
