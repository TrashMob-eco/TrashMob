namespace TrashMob.Shared.Tests
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Poco;
    using Xunit;

    public abstract class UpcomingEventsInYourAreaNotifierTestsBase : NotifierTestsBase
    {
        protected abstract INotificationEngine Engine { get; }
        protected abstract int NumberOfDaysToAddForEventOutOfWindow { get; }

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            // Arrange

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Never);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventForUser_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Once);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With2EventForUser_Sends2Emails()
        {
            // Arrange
            List<Event> events = GetEventList2();
            List<User> users = GetUserList1();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor2Users_Sends2Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList2();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoIsAlreadyAttending_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // The user is attending all available events
            EventAttendeeManager.Setup(ea => ea.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(events);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Never);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoIsAttendingOtherEventsButNotThisOne_SendsOneEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            List<Event> alternateEvents = GetEventList1();
            alternateEvents[0].Id = new Guid();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // The user is attending all available events
            EventAttendeeManager.Setup(ea => ea.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(alternateEvents);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Once);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasAlreadyReceivedTheEmail_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // The user has already received notifications for all events
            var userNotification = new UserNotification()
            {
                EventId = events[0].Id,
                UserId = users[0].Id,
                SentDate = DateTimeOffset.UtcNow.AddDays(-1),
                UserNotificationTypeId = (int)NotificationType,
            };

            var userNotifications = new List<UserNotification>() { userNotification };

            UserNotificationManager.Setup(ea => ea.GetCollectionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(userNotifications);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Never);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasNotAlreadyReceivedTheEmail_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            var userNotifications = new List<UserNotification>();

            UserNotificationManager.Setup(ea => ea.GetCollectionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(userNotifications);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Once);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhereEventIsMoreThanRequiredHoursAway_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].EventDate = DateTimeOffset.UtcNow.AddDays(NumberOfDaysToAddForEventOutOfWindow);

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Never);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhereEventIsOutsideOfTravelPreference_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].EventDate = DateTimeOffset.UtcNow.AddDays(3);

            EventManager.Setup(e => e.GetActiveEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            UserManager.Setup(u => u.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Setup a return of distance between User and Event of 50 (in whatever units)
            MapRepository.Setup(mr => mr.GetDistanceBetweenTwoPointsAsync(It.IsAny<Tuple<double, double>>(), It.IsAny<Tuple<double, double>>(), It.IsAny<bool>())).ReturnsAsync(50);

            // Act
            await Engine.GenerateNotificationsAsync();

            // Assert
            UserManager.Verify(_ => _.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventManager.Verify(_ => _.GetActiveEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventAttendeeManager.Verify(_ => _.GetEventsUserIsAttendingAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            UserNotificationManager.Verify(_ => _.AddAsync(It.IsAny<UserNotification>(), It.IsAny<CancellationToken>()), Times.Never);
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
