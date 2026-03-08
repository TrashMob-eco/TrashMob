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

    public class EventAttendeesV2ControllerTests
    {
        private readonly Mock<IEventAttendeeManager> eventAttendeeManager = new();
        private readonly Mock<ILogger<EventAttendeesV2Controller>> logger = new();
        private readonly EventAttendeesV2Controller controller;

        public EventAttendeesV2ControllerTests()
        {
            controller = new EventAttendeesV2Controller(eventAttendeeManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
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
    }
}
