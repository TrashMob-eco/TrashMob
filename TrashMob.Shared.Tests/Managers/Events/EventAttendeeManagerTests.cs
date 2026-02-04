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
    /// Unit tests for <see cref="EventAttendeeManager"/>.
    /// </summary>
    public class EventAttendeeManagerTests
    {
        private readonly Mock<IBaseRepository<EventAttendee>> _attendeeRepository;
        private readonly Mock<IKeyedRepository<Event>> _eventRepository;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly EventAttendeeManager _sut;

        public EventAttendeeManagerTests()
        {
            _attendeeRepository = new Mock<IBaseRepository<EventAttendee>>();
            _eventRepository = new Mock<IKeyedRepository<Event>>();
            _emailManager = new Mock<IEmailManager>();

            // Default setup for email manager
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new EventAttendeeManager(
                _attendeeRepository.Object,
                _eventRepository.Object,
                _emailManager.Object);
        }

        #region GetEventsUserIsAttendingAsync Tests

        [Fact]
        public async Task GetEventsUserIsAttendingAsync_ReturnsEventsUserIsAttending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId1 = Guid.NewGuid();
            var eventId2 = Guid.NewGuid();

            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId1).ForUser(userId).Build(),
                new EventAttendeeBuilder().ForEvent(eventId2).ForUser(userId).Build()
            };

            var event1 = new EventBuilder()
                .WithId(eventId1)
                .WithName("Beach Cleanup")
                .AsActive()
                .InTheFuture()
                .Build();

            var event2 = new EventBuilder()
                .WithId(eventId2)
                .WithName("Park Cleanup")
                .AsActive()
                .InTheFuture()
                .Build();

            var events = new List<Event> { event1, event2 };

            _attendeeRepository.SetupGetWithFilter(attendees);
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetEventsUserIsAttendingAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
        }

        [Fact]
        public async Task GetEventsUserIsAttendingAsync_ExcludesCanceledEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId1 = Guid.NewGuid();
            var eventId2 = Guid.NewGuid();

            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId1).ForUser(userId).Build(),
                new EventAttendeeBuilder().ForEvent(eventId2).ForUser(userId).Build()
            };

            var activeEvent = new EventBuilder()
                .WithId(eventId1)
                .WithName("Active Event")
                .AsActive()
                .Build();

            var canceledEvent = new EventBuilder()
                .WithId(eventId2)
                .WithName("Canceled Event")
                .AsCancelled()
                .Build();

            var events = new List<Event> { activeEvent, canceledEvent };

            _attendeeRepository.SetupGetWithFilter(attendees);
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetEventsUserIsAttendingAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Active Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetEventsUserIsAttendingAsync_WithFutureEventsOnly_ReturnsFutureEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId1 = Guid.NewGuid();
            var eventId2 = Guid.NewGuid();

            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId1).ForUser(userId).Build(),
                new EventAttendeeBuilder().ForEvent(eventId2).ForUser(userId).Build()
            };

            var futureEvent = new EventBuilder()
                .WithId(eventId1)
                .WithName("Future Event")
                .AsActive()
                .InTheFuture()
                .Build();

            var pastEvent = new EventBuilder()
                .WithId(eventId2)
                .WithName("Past Event")
                .AsActive()
                .InThePast()
                .Build();

            var events = new List<Event> { futureEvent, pastEvent };

            _attendeeRepository.SetupGetWithFilter(attendees);
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetEventsUserIsAttendingAsync(userId, futureEventsOnly: true);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Future Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetEventsUserIsAttendingAsync_WhenNotAttendingAny_ReturnsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var attendees = new List<EventAttendee>();

            _attendeeRepository.SetupGetWithFilter(attendees);

            // Act
            var result = await _sut.GetEventsUserIsAttendingAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetCanceledEventsUserIsAttendingAsync Tests

        [Fact]
        public async Task GetCanceledEventsUserIsAttendingAsync_ReturnsCanceledEventsOnly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId1 = Guid.NewGuid();
            var eventId2 = Guid.NewGuid();

            var attendees = new List<EventAttendee>
            {
                new EventAttendeeBuilder().ForEvent(eventId1).ForUser(userId).Build(),
                new EventAttendeeBuilder().ForEvent(eventId2).ForUser(userId).Build()
            };

            var activeEvent = new EventBuilder()
                .WithId(eventId1)
                .WithName("Active Event")
                .AsActive()
                .Build();

            var canceledEvent = new EventBuilder()
                .WithId(eventId2)
                .WithName("Canceled Event")
                .AsCancelled()
                .Build();

            var events = new List<Event> { activeEvent, canceledEvent };

            _attendeeRepository.SetupGetWithFilter(attendees);
            _eventRepository.SetupGetWithFilter(events);

            // Act
            var result = await _sut.GetCanceledEventsUserIsAttendingAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Canceled Event", resultList[0].Name);
        }

        #endregion

        #region IsEventLeadAsync Tests

        [Fact]
        public async Task IsEventLeadAsync_WhenUserIsLead_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsEventLead()
                .Build();

            var attendees = new List<EventAttendee> { attendee };
            _attendeeRepository.SetupGetWithFilter(attendees);

            // Act
            var result = await _sut.IsEventLeadAsync(eventId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEventLeadAsync_WhenUserIsEventCreator_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsRegularAttendee()
                .Build();

            var evt = new EventBuilder()
                .WithId(eventId)
                .CreatedBy(userId)
                .Build();

            var attendees = new List<EventAttendee> { attendee };
            _attendeeRepository.SetupGetWithFilter(attendees);
            _eventRepository.Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            // Act
            var result = await _sut.IsEventLeadAsync(eventId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEventLeadAsync_WhenUserIsNotLead_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsRegularAttendee()
                .Build();

            var evt = new EventBuilder()
                .WithId(eventId)
                .CreatedBy(creatorId)
                .Build();

            var attendees = new List<EventAttendee> { attendee };
            _attendeeRepository.SetupGetWithFilter(attendees);
            _eventRepository.Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            // Act
            var result = await _sut.IsEventLeadAsync(eventId, userId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetEventLeadsAsync Tests

        [Fact]
        public async Task GetEventLeadsAsync_ReturnsOnlyLeads()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var leadId = Guid.NewGuid();
            var regularId = Guid.NewGuid();

            var lead = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(leadId)
                .AsEventLead()
                .Build();

            var regularAttendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(regularId)
                .AsRegularAttendee()
                .Build();

            var attendees = new List<EventAttendee> { lead, regularAttendee };

            _attendeeRepository.SetupGetWithFilter(attendees);

            // Act
            var result = await _sut.GetEventLeadsAsync(eventId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(leadId, resultList[0].UserId);
        }

        #endregion

        #region PromoteToLeadAsync Tests

        [Fact]
        public async Task PromoteToLeadAsync_SetsUserAsLead()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var promoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();
            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(user)
                .AsRegularAttendee()
                .Build();

            var evt = new EventBuilder()
                .WithId(eventId)
                .WithName("Beach Cleanup")
                .Build();

            // No existing leads
            var emptyLeads = new List<EventAttendee>();
            var attendees = new List<EventAttendee> { attendee };

            // Setup for count query (no leads)
            _attendeeRepository.Setup(r => r.Get(It.Is<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(
                    expr => true), It.IsAny<bool>()))
                .Returns((System.Linq.Expressions.Expression<Func<EventAttendee, bool>> predicate, bool _) =>
                {
                    // Return empty for lead count, then attendee for lookup
                    return new TestAsyncEnumerable<EventAttendee>(
                        attendees.AsQueryable().Where(predicate.Compile()));
                });

            _attendeeRepository.Setup(r => r.UpdateAsync(It.IsAny<EventAttendee>()))
                .ReturnsAsync((EventAttendee ea) => ea);

            _eventRepository.Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            // Act
            var result = await _sut.PromoteToLeadAsync(eventId, userId, promoterUserId);

            // Assert
            Assert.True(result.IsEventLead);
            _attendeeRepository.Verify(r => r.UpdateAsync(It.Is<EventAttendee>(ea =>
                ea.UserId == userId && ea.IsEventLead == true)), Times.Once);
        }

        [Fact]
        public async Task PromoteToLeadAsync_WhenMaxLeadsReached_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var promoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            // 5 existing leads
            var existingLeads = Enumerable.Range(0, 5).Select(_ =>
                new EventAttendeeBuilder()
                    .ForEvent(eventId)
                    .AsEventLead()
                    .Build())
                .ToList();

            _attendeeRepository.SetupGetWithFilter(existingLeads);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.PromoteToLeadAsync(eventId, userId, promoterUserId));
        }

        [Fact]
        public async Task PromoteToLeadAsync_WhenUserNotAttendee_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var promoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var emptyAttendees = new List<EventAttendee>();
            _attendeeRepository.SetupGetWithFilter(emptyAttendees);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.PromoteToLeadAsync(eventId, userId, promoterUserId));
        }

        [Fact]
        public async Task PromoteToLeadAsync_WhenAlreadyLead_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var promoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsEventLead()
                .Build();

            var attendees = new List<EventAttendee> { attendee };
            _attendeeRepository.SetupGetWithFilter(attendees);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.PromoteToLeadAsync(eventId, userId, promoterUserId));
        }

        [Fact]
        public async Task PromoteToLeadAsync_SendsNotificationEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var promoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();
            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(user)
                .AsRegularAttendee()
                .Build();

            var evt = new EventBuilder()
                .WithId(eventId)
                .WithName("Beach Cleanup")
                .Build();

            var attendees = new List<EventAttendee> { attendee };

            _attendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns((System.Linq.Expressions.Expression<Func<EventAttendee, bool>> predicate, bool _) =>
                    new TestAsyncEnumerable<EventAttendee>(attendees.AsQueryable().Where(predicate.Compile())));

            _attendeeRepository.Setup(r => r.UpdateAsync(It.IsAny<EventAttendee>()))
                .ReturnsAsync((EventAttendee ea) => ea);

            _eventRepository.Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            // Act
            await _sut.PromoteToLeadAsync(eventId, userId, promoterUserId);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("co-lead")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Any(a => a.Email == "user@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DemoteFromLeadAsync Tests

        [Fact]
        public async Task DemoteFromLeadAsync_RemovesLeadStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var demoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var otherLeadId = Guid.NewGuid();

            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();
            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(user)
                .AsEventLead()
                .Build();

            var otherLead = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(otherLeadId)
                .AsEventLead()
                .Build();

            var evt = new EventBuilder()
                .WithId(eventId)
                .WithName("Beach Cleanup")
                .Build();

            var attendees = new List<EventAttendee> { attendee, otherLead };

            _attendeeRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<EventAttendee, bool>>>(), It.IsAny<bool>()))
                .Returns((System.Linq.Expressions.Expression<Func<EventAttendee, bool>> predicate, bool _) =>
                    new TestAsyncEnumerable<EventAttendee>(attendees.AsQueryable().Where(predicate.Compile())));

            _attendeeRepository.Setup(r => r.UpdateAsync(It.IsAny<EventAttendee>()))
                .ReturnsAsync((EventAttendee ea) => ea);

            _eventRepository.Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evt);

            // Act
            var result = await _sut.DemoteFromLeadAsync(eventId, userId, demoterUserId);

            // Assert
            Assert.False(result.IsEventLead);
            _attendeeRepository.Verify(r => r.UpdateAsync(It.Is<EventAttendee>(ea =>
                ea.UserId == userId && ea.IsEventLead == false)), Times.Once);
        }

        [Fact]
        public async Task DemoteFromLeadAsync_WhenLastLead_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var demoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var attendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsEventLead()
                .Build();

            // Only 1 lead
            var attendees = new List<EventAttendee> { attendee };
            _attendeeRepository.SetupGetWithFilter(attendees);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DemoteFromLeadAsync(eventId, userId, demoterUserId));
        }

        [Fact]
        public async Task DemoteFromLeadAsync_WhenUserNotLead_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var demoterUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var otherLeadId = Guid.NewGuid();

            var regularAttendee = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsRegularAttendee()
                .Build();

            var otherLead = new EventAttendeeBuilder()
                .ForEvent(eventId)
                .ForUser(otherLeadId)
                .AsEventLead()
                .Build();

            var attendees = new List<EventAttendee> { regularAttendee, otherLead };
            _attendeeRepository.SetupGetWithFilter(attendees);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DemoteFromLeadAsync(eventId, userId, demoterUserId));
        }

        #endregion
    }
}
