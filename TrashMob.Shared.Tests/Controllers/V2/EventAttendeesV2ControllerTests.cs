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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class EventAttendeesV2ControllerTests
    {
        private readonly Mock<IEventAttendeeManager> eventAttendeeManager = new();
        private readonly Mock<IUserWaiverManager> userWaiverManager = new();
        private readonly Mock<IDependentWaiverManager> dependentWaiverManager = new();
        private readonly Mock<IKeyedManager<User>> userManager = new();
        private readonly Mock<IEmailManager> emailManager = new();
        private readonly Mock<ILogger<EventAttendeesV2Controller>> logger = new();
        private readonly EventAttendeesV2Controller controller;
        private readonly Guid currentUserId = Guid.NewGuid();

        public EventAttendeesV2ControllerTests()
        {
            controller = new EventAttendeesV2Controller(
                eventAttendeeManager.Object,
                userWaiverManager.Object,
                dependentWaiverManager.Object,
                userManager.Object,
                emailManager.Object,
                logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = currentUserId.ToString();
        }

        [Fact]
        public async Task GetAttendeeCount_ReturnsOkWithCount()
        {
            var eventId = Guid.NewGuid();

            eventAttendeeManager
                .Setup(m => m.GetActiveAttendeeCountAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(15);

            var result = await controller.GetAttendeeCount(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventAttendeeCountDto>(okResult.Value);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(15, dto.Count);
        }

        [Fact]
        public async Task GetAttendeeCount_ReturnsZero_WhenNoAttendees()
        {
            var eventId = Guid.NewGuid();

            eventAttendeeManager
                .Setup(m => m.GetActiveAttendeeCountAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await controller.GetAttendeeCount(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventAttendeeCountDto>(okResult.Value);
            Assert.Equal(0, dto.Count);
        }

        [Fact]
        public async Task GetAttendees_ReturnsOkWithPagedResponse()
        {
            var eventId = Guid.NewGuid();
            var attendees = new List<EventAttendee>
            {
                new()
                {
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    SignUpDate = new DateTimeOffset(2026, 3, 1, 10, 0, 0, TimeSpan.Zero),
                    IsEventLead = true,
                    User = new User
                    {
                        UserName = "leaduser",
                        GivenName = "Lead",
                        ProfilePhotoUrl = "https://example.com/lead.jpg",
                    },
                },
                new()
                {
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    SignUpDate = new DateTimeOffset(2026, 3, 2, 10, 0, 0, TimeSpan.Zero),
                    IsEventLead = false,
                    User = new User
                    {
                        UserName = "volunteer1",
                        GivenName = "Alex",
                        ProfilePhotoUrl = "https://example.com/alex.jpg",
                    },
                },
            };

            var queryable = new TestAsyncEnumerable<EventAttendee>(attendees);
            var filter = new QueryParameters { Page = 1, PageSize = 25 };

            eventAttendeeManager
                .Setup(m => m.GetEventAttendeesQueryable(eventId))
                .Returns(queryable);

            var result = await controller.GetAttendees(eventId, filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<EventAttendeeDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal("leaduser", response.Items[0].UserName);
            Assert.True(response.Items[0].IsEventLead);
            Assert.Equal("volunteer1", response.Items[1].UserName);
            Assert.False(response.Items[1].IsEventLead);
        }

        [Fact]
        public async Task GetAttendees_ReturnsEmptyPagedResponse_WhenNoAttendees()
        {
            var eventId = Guid.NewGuid();
            var queryable = new TestAsyncEnumerable<EventAttendee>(Enumerable.Empty<EventAttendee>());
            var filter = new QueryParameters { Page = 1, PageSize = 25 };

            eventAttendeeManager
                .Setup(m => m.GetEventAttendeesQueryable(eventId))
                .Returns(queryable);

            var result = await controller.GetAttendees(eventId, filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<EventAttendeeDto>>(okResult.Value);
            Assert.Empty(response.Items);
            Assert.Equal(0, response.Pagination.TotalCount);
        }

        [Fact]
        public async Task AddEventAttendee_ReturnsOk_WhenWaiverValid()
        {
            var eventId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeDto { UserId = currentUserId };

            userWaiverManager
                .Setup(m => m.HasValidWaiverForEventAsync(currentUserId, eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.AddEventAttendee(eventId, attendeeDto, CancellationToken.None);

            Assert.IsType<OkResult>(result);
            eventAttendeeManager.Verify(
                m => m.AddAsync(It.Is<EventAttendee>(ea => ea.EventId == eventId), currentUserId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddEventAttendee_ReturnsBadRequest_WhenWaiverRequired()
        {
            var eventId = Guid.NewGuid();
            var attendeeDto = new EventAttendeeDto { UserId = currentUserId };
            var requiredWaivers = new List<WaiverVersion>
            {
                new() { Id = Guid.NewGuid() },
            };

            userWaiverManager
                .Setup(m => m.HasValidWaiverForEventAsync(currentUserId, eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            userWaiverManager
                .Setup(m => m.GetRequiredWaiversForEventAsync(currentUserId, eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(requiredWaivers);

            var result = await controller.AddEventAttendee(eventId, attendeeDto, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEventAttendee_ReturnsNoContent()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var result = await controller.DeleteEventAttendee(eventId, userId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            eventAttendeeManager.Verify(m => m.Delete(eventId, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetEventLeads_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            var leads = new List<EventAttendee>
            {
                new()
                {
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    IsEventLead = true,
                    User = new User { UserName = "lead1", City = "Seattle" },
                },
            };

            eventAttendeeManager
                .Setup(m => m.GetEventLeadsAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leads);

            var result = await controller.GetEventLeads(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<DisplayUser>>(okResult.Value);
            Assert.Single(users);
        }

        [Fact]
        public async Task PromoteToLead_ReturnsOk_WhenCurrentUserIsLead()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var promoted = new EventAttendee { EventId = eventId, UserId = userId, IsEventLead = true, User = new User { UserName = "promoted" } };

            eventAttendeeManager
                .Setup(m => m.IsEventLeadAsync(eventId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            eventAttendeeManager
                .Setup(m => m.PromoteToLeadAsync(eventId, userId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(promoted);

            var result = await controller.PromoteToLead(eventId, userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var attendee = Assert.IsType<EventAttendeeDto>(okResult.Value);
            Assert.True(attendee.IsEventLead);
        }

        [Fact]
        public async Task PromoteToLead_ReturnsForbid_WhenNotLead()
        {
            var eventId = Guid.NewGuid();

            eventAttendeeManager
                .Setup(m => m.IsEventLeadAsync(eventId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.PromoteToLead(eventId, Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DemoteFromLead_ReturnsBadRequest_WhenInvalidOperation()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            eventAttendeeManager
                .Setup(m => m.IsEventLeadAsync(eventId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            eventAttendeeManager
                .Setup(m => m.DemoteFromLeadAsync(eventId, userId, currentUserId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Cannot demote the last lead"));

            var result = await controller.DemoteFromLead(eventId, userId, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task VerifyWaiverStatus_ReturnsOk_WhenEventLead()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            eventAttendeeManager
                .Setup(m => m.IsEventLeadAsync(eventId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            userWaiverManager
                .Setup(m => m.HasValidWaiverForEventAsync(userId, eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.VerifyAttendeeWaiverStatus(eventId, userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var waiverResult = Assert.IsType<WaiverCheckResultDto>(okResult.Value);
            Assert.True(waiverResult.HasValidWaiver);
        }

        [Fact]
        public async Task VerifyWaiverStatus_ReturnsForbid_WhenNotLeadOrAdmin()
        {
            var eventId = Guid.NewGuid();

            eventAttendeeManager
                .Setup(m => m.IsEventLeadAsync(eventId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.VerifyAttendeeWaiverStatus(eventId, Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
