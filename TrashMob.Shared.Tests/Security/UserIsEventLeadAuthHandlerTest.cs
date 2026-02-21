namespace TrashMob.Shared.Tests.Security
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using Xunit;

    public class UserIsEventLeadAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<IEventAttendeeManager> _mockEventAttendeeManager;
        private readonly Mock<IKeyedManager<Event>> _mockEventManager;
        private readonly Mock<ILogger<UserIsEventLeadAuthHandler>> _mockLogger;
        private readonly UserIsEventLeadAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserIsEventLeadAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockEventAttendeeManager = new Mock<IEventAttendeeManager>();
            _mockEventManager = new Mock<IKeyedManager<Event>>();
            _mockLogger = new Mock<ILogger<UserIsEventLeadAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserIsEventLeadAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockEventAttendeeManager.Object,
                _mockEventManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_EventCreator_Succeeds()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("lead@test.com").Build();
            var evt = new Event { Id = eventId, CreatedByUserId = userId };

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("lead@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventManager.Setup(m => m.GetWithNoTrackingAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            var resource = new Event { Id = eventId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("lead@test.com");
            var requirement = new UserIsEventLeadRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_CoLead_Succeeds()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("colead@test.com").Build();
            var evt = new Event { Id = eventId, CreatedByUserId = otherUserId };

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("colead@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventManager.Setup(m => m.GetWithNoTrackingAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);
            _mockEventAttendeeManager.Setup(m => m.IsEventLeadAsync(eventId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resource = new Event { Id = eventId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("colead@test.com");
            var requirement = new UserIsEventLeadRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NonLead_DoesNotSucceed()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();
            var evt = new Event { Id = eventId, CreatedByUserId = otherUserId };

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventManager.Setup(m => m.GetWithNoTrackingAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);
            _mockEventAttendeeManager.Setup(m => m.IsEventLeadAsync(eventId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resource = new Event { Id = eventId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("user@test.com");
            var requirement = new UserIsEventLeadRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_EventNotFound_DoesNotSucceed()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("lead@test.com").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("lead@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventManager.Setup(m => m.GetWithNoTrackingAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var resource = new Event { Id = eventId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("lead@test.com");
            var requirement = new UserIsEventLeadRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var resource = new Event { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("missing@test.com");
            var requirement = new UserIsEventLeadRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_EventSummaryResource_ExtractsEventId()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("lead@test.com").Build();
            var evt = new Event { Id = eventId, CreatedByUserId = userId };

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("lead@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventManager.Setup(m => m.GetWithNoTrackingAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            var resource = new EventSummary { EventId = eventId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("lead@test.com");
            var requirement = new UserIsEventLeadRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }
    }
}
