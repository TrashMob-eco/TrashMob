namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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
    using Xunit;

    public class EventSummaryV2ControllerTests
    {
        private readonly Mock<IEventSummaryManager> eventSummaryManager = new();
        private readonly Mock<IKeyedManager<Event>> eventManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<EventSummaryV2Controller>> logger = new();
        private readonly EventSummaryV2Controller controller;

        private readonly Guid currentUserId = Guid.NewGuid();

        public EventSummaryV2ControllerTests()
        {
            controller = new EventSummaryV2Controller(eventSummaryManager.Object, eventManager.Object, authorizationService.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(
                        new System.Security.Claims.ClaimsIdentity(
                            new[] { new System.Security.Claims.Claim("sub", currentUserId.ToString()) }, "test")),
                },
            };
            controller.HttpContext.Items["UserId"] = currentUserId.ToString();
        }

        [Fact]
        public async Task GetEventSummary_ReturnsSummary_WhenExists()
        {
            var eventId = Guid.NewGuid();
            var summary = new EventSummary
            {
                EventId = eventId,
                NumberOfBuckets = 3,
                NumberOfBags = 10,
                DurationInMinutes = 90,
                ActualNumberOfAttendees = 6,
                PickedWeight = 25.5m,
                PickedWeightUnitId = 1,
                IsFromRouteData = false,
                Notes = "Good turnout",
                CreatedDate = new DateTimeOffset(2026, 3, 1, 10, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.Zero),
            };

            eventSummaryManager
                .Setup(m => m.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<EventSummary, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventSummary> { summary });

            var result = await controller.GetEventSummary(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventSummaryDto>(okResult.Value);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(3, dto.NumberOfBuckets);
            Assert.Equal(10, dto.NumberOfBags);
            Assert.Equal(90, dto.DurationInMinutes);
            Assert.Equal(6, dto.ActualNumberOfAttendees);
            Assert.Equal(25.5m, dto.PickedWeight);
            Assert.Equal(1, dto.PickedWeightUnitId);
            Assert.False(dto.IsFromRouteData);
            Assert.Equal("Good turnout", dto.Notes);
        }

        [Fact]
        public async Task GetEventSummary_ReturnsEmptySummary_WhenNoneExists()
        {
            var eventId = Guid.NewGuid();

            eventSummaryManager
                .Setup(m => m.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<EventSummary, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventSummary>());

            var result = await controller.GetEventSummary(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventSummaryDto>(okResult.Value);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(0, dto.NumberOfBags);
            Assert.Equal(0, dto.NumberOfBuckets);
            Assert.Equal(0, dto.DurationInMinutes);
            Assert.Equal(0, dto.ActualNumberOfAttendees);
            Assert.Equal(0m, dto.PickedWeight);
        }

        [Fact]
        public async Task AddEventSummary_ReturnsCreated_WhenAuthorized()
        {
            var eventId = Guid.NewGuid();
            var summaryDto = new EventSummaryDto { NumberOfBags = 5 };
            var created = new EventSummary { EventId = eventId, NumberOfBags = 5 };

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            eventSummaryManager
                .Setup(m => m.AddAsync(It.IsAny<EventSummary>(), currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await controller.AddEventSummary(eventId, summaryDto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task AddEventSummary_ReturnsForbid_WhenNotAuthorized()
        {
            var eventId = Guid.NewGuid();
            var summaryDto = new EventSummaryDto { NumberOfBags = 5 };

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.AddEventSummary(eventId, summaryDto, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteEventSummary_ReturnsNoContent_WhenAuthorized()
        {
            var eventId = Guid.NewGuid();
            var mobEvent = new Event { Id = eventId };

            eventManager
                .Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var result = await controller.DeleteEventSummary(eventId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            eventSummaryManager.Verify(m => m.DeleteAsync(eventId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
