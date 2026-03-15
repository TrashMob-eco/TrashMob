namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
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

    public class EventsV2ControllerTests
    {
        private readonly Mock<IEventManager> eventManager = new();
        private readonly Mock<IEventAttendeeManager> eventAttendeeManager = new();
        private readonly Mock<IEventSummaryManager> eventSummaryManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<EventsV2Controller>> logger = new();
        private readonly EventsV2Controller controller;

        public EventsV2ControllerTests()
        {
            controller = new EventsV2Controller(eventManager.Object, eventAttendeeManager.Object, eventSummaryManager.Object, authorizationService.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetEvents_ReturnsOkWithPagedResponse()
        {
            var events = new List<Event>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Park Cleanup",
                    EventDate = DateTimeOffset.UtcNow.AddDays(1),
                    EventStatusId = (int)EventStatusEnum.Active,
                    EventVisibilityId = (int)EventVisibilityEnum.Public,
                    CreatedByUserId = Guid.NewGuid(),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Beach Cleanup",
                    EventDate = DateTimeOffset.UtcNow.AddDays(2),
                    EventStatusId = (int)EventStatusEnum.Active,
                    EventVisibilityId = (int)EventVisibilityEnum.Public,
                    CreatedByUserId = Guid.NewGuid(),
                },
            };

            var queryable = new TestAsyncEnumerable<Event>(events);
            var filter = new EventQueryParameters { Page = 1, PageSize = 25 };

            eventManager
                .Setup(m => m.GetFilteredEventsQueryableAsync(filter, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryable);

            var result = await controller.GetEvents(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<EventDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal(2, response.Pagination.TotalCount);
            Assert.Equal("Park Cleanup", response.Items[0].Name);
            Assert.Equal("Beach Cleanup", response.Items[1].Name);
        }

        [Fact]
        public async Task GetEvents_ReturnsEmptyPagedResponse_WhenNoEvents()
        {
            var queryable = new TestAsyncEnumerable<Event>(Enumerable.Empty<Event>());
            var filter = new EventQueryParameters { Page = 1, PageSize = 25 };

            eventManager
                .Setup(m => m.GetFilteredEventsQueryableAsync(filter, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryable);

            var result = await controller.GetEvents(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<EventDto>>(okResult.Value);
            Assert.Empty(response.Items);
            Assert.Equal(0, response.Pagination.TotalCount);
        }

        [Fact]
        public async Task GetEvents_AppliesPagination()
        {
            var events = Enumerable.Range(1, 5).Select(i => new Event
            {
                Id = Guid.NewGuid(),
                Name = $"Event {i}",
                EventDate = DateTimeOffset.UtcNow.AddDays(i),
                EventStatusId = (int)EventStatusEnum.Active,
                EventVisibilityId = (int)EventVisibilityEnum.Public,
                CreatedByUserId = Guid.NewGuid(),
            }).ToList();

            var queryable = new TestAsyncEnumerable<Event>(events);
            var filter = new EventQueryParameters { Page = 2, PageSize = 2 };

            eventManager
                .Setup(m => m.GetFilteredEventsQueryableAsync(filter, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryable);

            var result = await controller.GetEvents(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<EventDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal(5, response.Pagination.TotalCount);
            Assert.Equal(2, response.Pagination.Page);
            Assert.True(response.Pagination.HasPrevious);
            Assert.True(response.Pagination.HasNext);
        }

        [Fact]
        public async Task GetEvent_ReturnsOkWithEventDto()
        {
            var eventId = Guid.NewGuid();
            var mobEvent = new Event
            {
                Id = eventId,
                Name = "Trail Cleanup",
                Description = "Clean up the hiking trail",
                EventDate = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
                DurationHours = 3,
                DurationMinutes = 0,
                EventTypeId = 1,
                EventStatusId = (int)EventStatusEnum.Active,
                City = "Seattle",
                Region = "WA",
                Country = "US",
                EventVisibilityId = (int)EventVisibilityEnum.Public,
                CreatedByUserId = Guid.NewGuid(),
            };

            eventManager
                .Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);

            var result = await controller.GetEvent(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventDto>(okResult.Value);
            Assert.Equal(eventId, dto.Id);
            Assert.Equal("Trail Cleanup", dto.Name);
            Assert.Equal("Seattle", dto.City);
            Assert.True(dto.IsEventPublic);
        }

        [Fact]
        public async Task GetEvent_ReturnsNotFound_WhenEventDoesNotExist()
        {
            var eventId = Guid.NewGuid();

            eventManager
                .Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var result = await controller.GetEvent(eventId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddEvent_ReturnsCreated()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();

            var eventDto = new EventDto { Id = Guid.NewGuid(), Name = "New Cleanup" };
            eventManager.Setup(m => m.AddAsync(It.IsAny<Event>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event { Id = eventDto.Id, Name = eventDto.Name });

            var result = await controller.AddEvent(eventDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(EventsV2Controller.GetEvent), createdResult.ActionName);
        }

        [Fact]
        public async Task UpdateEvent_ReturnsForbid_WhenNotAuthorized()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();

            authorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(),
                    It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var eventDto = new EventDto { Id = Guid.NewGuid(), Name = "Cleanup" };

            var result = await controller.UpdateEvent(eventDto, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_ReturnsNoContent_WhenAuthorized()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            controller.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(
                    new[] { new System.Security.Claims.Claim("sub", Guid.NewGuid().ToString()) }, "test"));

            var eventId = Guid.NewGuid();
            var mobEvent = new Event { Id = eventId };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);
            authorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(),
                    It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            eventManager.Setup(m => m.DeleteAsync(eventId, It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var request = new TrashMob.Shared.Poco.EventCancellationRequest { EventId = eventId, CancellationReason = "Rain" };

            var result = await controller.DeleteEvent(request, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetUserEvents_ReturnsOk()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            eventManager.Setup(m => m.GetUserEventsAsync(userId, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Event>());
            eventAttendeeManager.Setup(m => m.GetEventsUserIsAttendingAsync(userId, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Event>());

            var result = await controller.GetUserEvents(userId, true, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEventsUserIsAttending_ReturnsOk()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            eventAttendeeManager.Setup(m => m.GetEventsUserIsAttendingAsync(userId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Event>());

            var result = await controller.GetEventsUserIsAttending(userId, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEventLocationsByTimeRange_ReturnsOk()
        {
            var start = DateTimeOffset.UtcNow.AddDays(-30);
            var end = DateTimeOffset.UtcNow;

            eventManager.Setup(m => m.GetEventLocationsByTimeRangeAsync(start, end, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Models.Poco.Location>());

            var result = await controller.GetEventLocationsByTimeRange(start, end, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
