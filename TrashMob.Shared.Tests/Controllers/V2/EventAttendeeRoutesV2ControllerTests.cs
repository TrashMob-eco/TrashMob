namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using NetTopologySuite.Geometries;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class EventAttendeeRoutesV2ControllerTests
    {
        private static readonly Geometry TestPath = new GeometryFactory()
            .CreateLineString(new[] { new Coordinate(-122.0, 47.0), new Coordinate(-122.1, 47.1) });

        private readonly Mock<IEventAttendeeRouteManager> routeManager = new();
        private readonly Mock<ILogger<EventAttendeeRoutesV2Controller>> logger = new();
        private readonly EventAttendeeRoutesV2Controller controller;

        public EventAttendeeRoutesV2ControllerTests()
        {
            controller = new EventAttendeeRoutesV2Controller(routeManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetByEventAndUser_ReturnsOk_WithRoutes()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var routes = new List<EventAttendeeRoute>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    CreatedByUserId = userId,
                    UserId = userId,
                    TotalDistanceMeters = 1000,
                    UserPath = TestPath,
                },
            };

            routeManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(routes);

            var result = await controller.GetByEventAndUser(eventId, userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<DisplayEventAttendeeRoute>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetByEvent_ReturnsOk_FilteringPrivateRoutes()
        {
            var eventId = Guid.NewGuid();
            var routes = new List<EventAttendeeRoute>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    CreatedByUserId = Guid.NewGuid(),
                    PrivacyLevel = "Public",
                    TotalDistanceMeters = 500,
                    UserPath = TestPath,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    CreatedByUserId = Guid.NewGuid(),
                    PrivacyLevel = "Private",
                    TotalDistanceMeters = 300,
                    UserPath = TestPath,
                },
            };

            routeManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(routes);

            var result = await controller.GetByEvent(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<DisplayEventAttendeeRoute>>(okResult.Value);
            Assert.Single(dtos); // Private route filtered out for unauthenticated user
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoRoutes()
        {
            routeManager.Setup(m => m.GetAsync(It.IsAny<Expression<Func<EventAttendeeRoute, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventAttendeeRoute>());

            var result = await controller.Get(Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var routeId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();

            var result = await controller.Delete(routeId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            routeManager.Verify(m => m.DeleteAsync(routeId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByUser_ReturnsOk_WithRoutes()
        {
            var userId = Guid.NewGuid();
            var routes = new List<EventAttendeeRoute>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EventId = Guid.NewGuid(),
                    CreatedByUserId = userId,
                    UserId = userId,
                    TotalDistanceMeters = 2000,
                    UserPath = TestPath,
                },
            };

            routeManager.Setup(m => m.GetByCreatedUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(routes);

            var result = await controller.GetByUser(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRoutes = Assert.IsAssignableFrom<IEnumerable<DisplayEventAttendeeRoute>>(okResult.Value);
            Assert.Single(returnedRoutes);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var displayRoute = new DisplayEventAttendeeRoute
            {
                Id = routeId,
                EventId = Guid.NewGuid(),
            };
            var updatedRoute = new EventAttendeeRoute { Id = routeId, UserPath = TestPath };

            routeManager
                .Setup(m => m.UpdateAsync(It.IsAny<EventAttendeeRoute>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedRoute);

            var result = await controller.Update(displayRoute, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<DisplayEventAttendeeRoute>(okResult.Value);
        }

        [Fact]
        public async Task UpdateRouteMetadata_ReturnsOk_WhenSuccessful()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var request = new UpdateRouteMetadataRequest { Notes = "Picked up 3 bags" };
            var route = new EventAttendeeRoute { Id = routeId, UserPath = TestPath };

            routeManager
                .Setup(m => m.UpdateRouteMetadataAsync(routeId, userId, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Success(route));

            var result = await controller.UpdateRouteMetadata(routeId, request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<DisplayEventAttendeeRoute>(okResult.Value);
        }

        [Fact]
        public async Task UpdateRouteMetadata_ReturnsNotFound_WhenRouteNotFound()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var request = new UpdateRouteMetadataRequest();

            routeManager
                .Setup(m => m.UpdateRouteMetadataAsync(routeId, userId, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Failure("Route not found."));

            var result = await controller.UpdateRouteMetadata(routeId, request, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateRouteMetadata_ReturnsBadRequest_WhenValidationFails()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var request = new UpdateRouteMetadataRequest();

            routeManager
                .Setup(m => m.UpdateRouteMetadataAsync(routeId, userId, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Failure("You do not own this route."));

            var result = await controller.UpdateRouteMetadata(routeId, request, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task TrimRouteTime_ReturnsOk_WhenSuccessful()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var request = new TrimRouteTimeRequest { NewEndTime = DateTimeOffset.UtcNow };
            var route = new EventAttendeeRoute { Id = routeId, UserPath = TestPath };

            routeManager
                .Setup(m => m.TrimRouteTimeAsync(routeId, userId, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Success(route));

            var result = await controller.TrimRouteTime(routeId, request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<DisplayEventAttendeeRoute>(okResult.Value);
        }

        [Fact]
        public async Task TrimRouteTime_ReturnsNotFound_WhenRouteNotFound()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var request = new TrimRouteTimeRequest { NewEndTime = DateTimeOffset.UtcNow };

            routeManager
                .Setup(m => m.TrimRouteTimeAsync(routeId, userId, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Failure("Route not found."));

            var result = await controller.TrimRouteTime(routeId, request, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task RestoreRouteTime_ReturnsOk_WhenSuccessful()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var route = new EventAttendeeRoute { Id = routeId, UserPath = TestPath };

            routeManager
                .Setup(m => m.RestoreRouteTimeAsync(routeId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Success(route));

            var result = await controller.RestoreRouteTime(routeId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<DisplayEventAttendeeRoute>(okResult.Value);
        }

        [Fact]
        public async Task RestoreRouteTime_ReturnsNotFound_WhenRouteNotFound()
        {
            var routeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            routeManager
                .Setup(m => m.RestoreRouteTimeAsync(routeId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeRoute>.Failure("Route not found."));

            var result = await controller.RestoreRouteTime(routeId, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}
