namespace TrashMob.Shared.Tests
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using Xunit;

    public abstract class UpcomingEventHostingNotifierTestsBase : NotifierTestsBase
    {
        protected abstract INotificationEngine Engine { get; }

        protected abstract int NumberOfDaysToAddForEventOutOfWindow { get; }

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            // Arrange

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventForUser_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);
            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Once);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With2EventForUser_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList2();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;
            events[1].CreatedByUserId = users[0].Id;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);
            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Exactly(2));
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor2Users_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList2();
            events[0].CreatedByUserId = users[0].Id;

            // The users are attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);
            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Exactly(2));
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Exactly(2));
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Once);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoIsNotHost_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoIsHostingOtherEventsButNotThisOne_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            List<Event> alternateEvents = GetEventList1();
            alternateEvents[0].Id = new Guid();
            alternateEvents[0].CreatedByUserId = users[0].Id;

            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasAlreadyReceivedTheEmail_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);
            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // The user has already received notifications for all events
            var userNotification = new UserNotification()
            {
                EventId = events[0].Id,
                UserId = users[0].Id,
                SentDate = DateTimeOffset.UtcNow.AddDays(-1),
                UserNotificationTypeId = (int)NotificationType,
            };

            var userNotifications = new List<UserNotification>() { userNotification };

            UserNotificationRepository.Setup(ea => ea.GetUserNotifications(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(userNotifications);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasNotAlreadyReceivedTheEmail_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);
            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            var userNotifications = new List<UserNotification>();

            UserNotificationRepository.Setup(ea => ea.GetUserNotifications(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(userNotifications);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Once);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasOptedOutOfAllEmails_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;

            users[0].IsOptedOutOfAllEmails = true;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);
            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Never);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Never);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
         public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasOptedOutOfThisEmail_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);

            List<UserNotificationPreference> userNotificationPreferences = GetUserNotificationPreferences();
            var unIndex = userNotificationPreferences.FindIndex(unp => unp.UserNotificationTypeId == (int)NotificationType);
            userNotificationPreferences[unIndex].IsOptedOut = true;

            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);
            UserNotificationPreferenceRepository.Setup(unp => unp.GetUserNotificationPreferences(It.IsAny<Guid>())).ReturnsAsync(userNotificationPreferences);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Never);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhereEventIsMoreThanRequiredHoursAway_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            events[0].CreatedByUserId = users[0].Id;

            // The user is attending all available events
            EventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(events);

            events[0].EventDate = DateTimeOffset.UtcNow.AddDays(NumberOfDaysToAddForEventOutOfWindow);

            EventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            UserRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // Act
            await Engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            UserRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            EventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            UserNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            UserNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            EmailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
