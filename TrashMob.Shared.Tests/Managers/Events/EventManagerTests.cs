namespace TrashMob.Shared.Tests.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Events;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="EventManager"/>.
    /// </summary>
    public class EventManagerTests
    {
        private readonly Mock<IKeyedRepository<Event>> _eventRepository;
        private readonly Mock<IEventAttendeeManager> _eventAttendeeManager;
        private readonly Mock<IBaseRepository<EventAttendee>> _eventAttendeeRepository;
        private readonly Mock<IEventLitterReportManager> _eventLitterReportManager;
        private readonly Mock<IMapManager> _mapManager;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly EventManager _sut;

        public EventManagerTests()
        {
            _eventRepository = new Mock<IKeyedRepository<Event>>();
            _eventAttendeeManager = new Mock<IEventAttendeeManager>();
            _eventAttendeeRepository = new Mock<IBaseRepository<EventAttendee>>();
            _eventLitterReportManager = new Mock<IEventLitterReportManager>();
            _mapManager = new Mock<IMapManager>();
            _emailManager = new Mock<IEmailManager>();

            // Default setup for common dependencies
            // Return the event date formatted as ISO 8601 string (parseable as DateTimeOffset)
            _mapManager.Setup(m => m.GetTimeForPointAsync(It.IsAny<Tuple<double, double>>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync((Tuple<double, double> _, DateTimeOffset eventDate) => eventDate.ToString("o"));
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new EventManager(
                _eventRepository.Object,
                _eventAttendeeManager.Object,
                _eventAttendeeRepository.Object,
                _eventLitterReportManager.Object,
                _mapManager.Object,
                _emailManager.Object);
        }

        #region GetActiveEventsAsync Tests

        [Fact]
        public async Task GetActiveEventsAsync_ReturnsOnlyActivePublicFutureEvents()
        {
            // Arrange
            var activeEvent = new EventBuilder()
                .WithName("Active Event")
                .AsActive()
                .InTheFuture()
                .AsPublic()
                .Build();

            var canceledEvent = new EventBuilder()
                .WithName("Canceled Event")
                .AsCancelled()
                .InTheFuture()
                .AsPublic()
                .Build();

            var privateEvent = new EventBuilder()
                .WithName("Private Event")
                .AsActive()
                .InTheFuture()
                .AsPrivate()
                .Build();

            var pastEvent = new EventBuilder()
                .WithName("Past Event")
                .AsActive()
                .InThePast()
                .AsPublic()
                .Build();

            var events = new List<Event> { activeEvent, canceledEvent, privateEvent, pastEvent };
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetActiveEventsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Active Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetActiveEventsAsync_ReturnsFullEvents()
        {
            // Arrange
            var fullEvent = new EventBuilder()
                .WithName("Full Event")
                .AsFull()
                .InTheFuture()
                .AsPublic()
                .Build();

            var events = new List<Event> { fullEvent };
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetActiveEventsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Full Event", result.First().Name);
        }

        [Fact]
        public async Task GetActiveEventsAsync_ReturnsEmpty_WhenNoActiveEvents()
        {
            // Arrange
            var events = new List<Event>();
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetActiveEventsAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetCompletedEventsAsync Tests

        [Fact]
        public async Task GetCompletedEventsAsync_ReturnsPastNonCanceledEvents()
        {
            // Arrange
            var completedEvent = new EventBuilder()
                .WithName("Completed Event")
                .AsActive()
                .InThePast()
                .Build();

            var canceledPastEvent = new EventBuilder()
                .WithName("Canceled Past Event")
                .AsCancelled()
                .InThePast()
                .Build();

            var futureEvent = new EventBuilder()
                .WithName("Future Event")
                .AsActive()
                .InTheFuture()
                .Build();

            var events = new List<Event> { completedEvent, canceledPastEvent, futureEvent };
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetCompletedEventsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Completed Event", resultList[0].Name);
        }

        #endregion

        #region GetUserEventsAsync Tests

        [Fact]
        public async Task GetUserEventsAsync_ReturnsEventsCreatedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var userEvent = new EventBuilder()
                .WithName("User's Event")
                .CreatedBy(userId)
                .AsActive()
                .Build();

            var otherUserEvent = new EventBuilder()
                .WithName("Other User's Event")
                .CreatedBy(otherUserId)
                .AsActive()
                .Build();

            var canceledUserEvent = new EventBuilder()
                .WithName("User's Canceled Event")
                .CreatedBy(userId)
                .AsCancelled()
                .Build();

            var events = new List<Event> { userEvent, otherUserEvent, canceledUserEvent };
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetUserEventsAsync(userId, false);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("User's Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetUserEventsAsync_WithFutureEventsOnly_ReturnsFutureEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var futureUserEvent = new EventBuilder()
                .WithName("Future Event")
                .CreatedBy(userId)
                .AsActive()
                .InTheFuture()
                .Build();

            var pastUserEvent = new EventBuilder()
                .WithName("Past Event")
                .CreatedBy(userId)
                .AsActive()
                .InThePast()
                .Build();

            var events = new List<Event> { futureUserEvent, pastUserEvent };
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetUserEventsAsync(userId, true);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Future Event", resultList[0].Name);
        }

        #endregion

        #region GetCanceledUserEventsAsync Tests

        [Fact]
        public async Task GetCanceledUserEventsAsync_ReturnsCanceledEventsCreatedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var canceledUserEvent = new EventBuilder()
                .WithName("Canceled Event")
                .CreatedBy(userId)
                .AsCancelled()
                .Build();

            var activeUserEvent = new EventBuilder()
                .WithName("Active Event")
                .CreatedBy(userId)
                .AsActive()
                .Build();

            var events = new List<Event> { canceledUserEvent, activeUserEvent };
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetCanceledUserEventsAsync(userId, false);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Canceled Event", resultList[0].Name);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_CreatesEventAndRegistersCreatorAsAttendee()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newEvent = new EventBuilder()
                .WithName("New Beach Cleanup")
                .CreatedBy(userId)
                .Build();

            _eventRepository.SetupAddAsync();
            _eventAttendeeManager.Setup(m => m.AddAsync(It.IsAny<EventAttendee>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventAttendee ea, Guid _, CancellationToken _) => ea);

            // Act
            var result = await _sut.AddAsync(newEvent, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Beach Cleanup", result.Name);
            _eventAttendeeManager.Verify(m => m.AddAsync(
                It.Is<EventAttendee>(ea => ea.UserId == userId && ea.EventId == newEvent.Id),
                userId,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_SendsNewEventNotificationEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newEvent = new EventBuilder()
                .WithName("Community Cleanup")
                .CreatedBy(userId)
                .Build();

            _eventRepository.SetupAddAsync();
            _eventAttendeeManager.Setup(m => m.AddAsync(It.IsAny<EventAttendee>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventAttendee ea, Guid _, CancellationToken _) => ea);

            // Act
            await _sut.AddAsync(newEvent, userId);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "New Event Alert",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_SetsEventStatusToCanceled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var existingEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Event to Cancel")
                .CreatedBy(userId)
                .AsActive()
                .Build();

            _eventRepository.SetupGetAsync(existingEvent);
            _eventRepository.SetupUpdateAsync();
            _eventLitterReportManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventLitterReport>());

            var emptyAttendees = new TestAsyncEnumerable<EventAttendee>(new List<EventAttendee>());
            _eventAttendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns(emptyAttendees);

            // Act
            var result = await _sut.DeleteAsync(eventId, "Event canceled due to weather", userId, CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
            _eventRepository.Verify(r => r.UpdateAsync(It.Is<Event>(e =>
                e.Id == eventId &&
                e.EventStatusId == (int)EventStatusEnum.Canceled &&
                e.CancellationReason == "Event canceled due to weather")), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_RemovesAssociatedLitterReports()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var litterReportId1 = Guid.NewGuid();
            var litterReportId2 = Guid.NewGuid();

            var existingEvent = new EventBuilder()
                .WithId(eventId)
                .CreatedBy(userId)
                .AsActive()
                .Build();

            var litterReports = new List<EventLitterReport>
            {
                new EventLitterReport { EventId = eventId, LitterReportId = litterReportId1 },
                new EventLitterReport { EventId = eventId, LitterReportId = litterReportId2 }
            };

            _eventRepository.SetupGetAsync(existingEvent);
            _eventRepository.SetupUpdateAsync();
            _eventLitterReportManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(litterReports);
            _eventLitterReportManager.Setup(m => m.Delete(eventId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var emptyAttendees = new TestAsyncEnumerable<EventAttendee>(new List<EventAttendee>());
            _eventAttendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns(emptyAttendees);

            // Act
            await _sut.DeleteAsync(eventId, "Cleanup complete", userId, CancellationToken.None);

            // Assert
            _eventLitterReportManager.Verify(m => m.Delete(eventId, litterReportId1, It.IsAny<CancellationToken>()), Times.Once);
            _eventLitterReportManager.Verify(m => m.Delete(eventId, litterReportId2, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SendsCancellationEmailToAllAttendees()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var attendeeId1 = Guid.NewGuid();
            var attendeeId2 = Guid.NewGuid();

            var existingEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Beach Cleanup")
                .CreatedBy(userId)
                .AsActive()
                .Build();

            var user1 = new UserBuilder().WithId(attendeeId1).WithEmail("user1@test.com").Build();
            var user2 = new UserBuilder().WithId(attendeeId2).WithEmail("user2@test.com").Build();

            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId).ForUser(user1).Build(),
                new EventAttendeeBuilder().ForEvent(eventId).ForUser(user2).Build()
            };
            attendees[0].User = user1;
            attendees[1].User = user2;

            _eventRepository.SetupGetAsync(existingEvent);
            _eventRepository.SetupUpdateAsync();
            _eventLitterReportManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventLitterReport>());

            var mockQueryable = new TestAsyncEnumerable<EventAttendee>(attendees);
            _eventAttendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns(mockQueryable);

            // Act
            await _sut.DeleteAsync(eventId, "Weather issues", userId, CancellationToken.None);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "A TrashMob.eco event you were scheduled to attend has been cancelled!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(a => a.Email == "user1@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);

            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "A TrashMob.eco event you were scheduled to attend has been cancelled!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(a => a.Email == "user2@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithNoSignificantChanges_DoesNotSendEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var existingEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Park Cleanup")
                .CreatedBy(userId)
                .InCity("Seattle")
                .Build();

            var updatedEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Park Cleanup - Updated Description")
                .CreatedBy(userId)
                .InCity("Seattle")
                .Build();
            updatedEvent.EventDate = existingEvent.EventDate;
            updatedEvent.Country = existingEvent.Country;
            updatedEvent.Region = existingEvent.Region;
            updatedEvent.PostalCode = existingEvent.PostalCode;
            updatedEvent.StreetAddress = existingEvent.StreetAddress;

            _eventRepository.SetupGetWithNoTrackingAsync(existingEvent);
            _eventRepository.SetupUpdateAsync();

            // Act
            await _sut.UpdateAsync(updatedEvent, userId);

            // Assert - No email should be sent for just name/description changes
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithDateChange_SendsUpdateEmailToAttendees()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var attendeeId = Guid.NewGuid();

            var existingEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Park Cleanup")
                .CreatedBy(userId)
                .Build();

            var updatedEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Park Cleanup")
                .CreatedBy(userId)
                .Build();
            updatedEvent.EventDate = existingEvent.EventDate.AddDays(1); // Date changed

            var user = new UserBuilder().WithId(attendeeId).WithEmail("attendee@test.com").Build();
            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId).ForUser(user).Build()
            };
            attendees[0].User = user;

            _eventRepository.SetupGetWithNoTrackingAsync(existingEvent);
            _eventRepository.SetupUpdateAsync();

            var mockQueryable = new TestAsyncEnumerable<EventAttendee>(attendees);
            _eventAttendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns(mockQueryable);

            // Act
            await _sut.UpdateAsync(updatedEvent, userId);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "A TrashMob.eco event you were scheduled to attend has been updated!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(a => a.Email == "attendee@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithLocationChange_SendsUpdateEmailToAttendees()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var attendeeId = Guid.NewGuid();

            var existingEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Park Cleanup")
                .CreatedBy(userId)
                .InCity("Seattle")
                .Build();

            var updatedEvent = new EventBuilder()
                .WithId(eventId)
                .WithName("Park Cleanup")
                .CreatedBy(userId)
                .InCity("Bellevue") // City changed
                .Build();
            updatedEvent.EventDate = existingEvent.EventDate;

            var user = new UserBuilder().WithId(attendeeId).WithEmail("attendee@test.com").Build();
            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId).ForUser(user).Build()
            };
            attendees[0].User = user;

            _eventRepository.SetupGetWithNoTrackingAsync(existingEvent);
            _eventRepository.SetupUpdateAsync();

            var mockQueryable = new TestAsyncEnumerable<EventAttendee>(attendees);
            _eventAttendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns(mockQueryable);

            // Act
            await _sut.UpdateAsync(updatedEvent, userId);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "A TrashMob.eco event you were scheduled to attend has been updated!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
