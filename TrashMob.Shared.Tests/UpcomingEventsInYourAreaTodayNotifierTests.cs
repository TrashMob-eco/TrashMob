namespace TrashMob.Shared.Tests
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using Xunit;

    public class UpcomingEventsInYourAreaTodayNotifierTests : NotifierTestsBase
    {
        private readonly Mock<IEventRepository> eventRepository;
        private readonly Mock<IEventAttendeeRepository> eventAttendeeRepository;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IUserNotificationRepository> userNotificationRepository;
        private readonly Mock<IUserNotificationPreferenceRepository> userNotificationPreferenceRepository;
        private readonly Mock<IEmailSender> emailSender;

        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaToday;

        public UpcomingEventsInYourAreaTodayNotifierTests()
        {
            eventRepository = new Mock<IEventRepository>();
            eventAttendeeRepository = new Mock<IEventAttendeeRepository>();
            userRepository = new Mock<IUserRepository>();
            userNotificationRepository = new Mock<IUserNotificationRepository>();
            userNotificationPreferenceRepository = new Mock<IUserNotificationPreferenceRepository>();
            emailSender = new Mock<IEmailSender>();
        }

        [Fact]
        public void GetEmailTemplate_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            var template = engine.GetEmailTemplate();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(template));
            Assert.Contains("There are upcoming TrashMob.eco events in your area today!", template);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventForUser_Sends1Email()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var createdById = Guid.NewGuid();

            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Once);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With2EventForUser_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList2();
            List<User> users = GetUserList1();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Exactly(2));
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor2Users_Sends2Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList2();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Exactly(2));
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Exactly(2));
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Exactly(2));
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Exactly(2));
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoIsAlreadyAttending_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // The user is attending all available events
            eventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>())).ReturnsAsync(events);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoIsAttendingOtherEventsButNotThisOne_SendsOneEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            List<Event> alternateEvents = GetEventList1();
            alternateEvents[0].Id = new Guid();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // The user is attending all available events
            eventAttendeeRepository.Setup(ea => ea.GetEventsUserIsAttending(It.IsAny<Guid>())).ReturnsAsync(alternateEvents);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Once);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasAlreadyReceivedTheEmail_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // The user has already received notifications for all events
            var userNotification = new UserNotification()
            {
                EventId = events[0].Id,
                UserId = users[0].Id,
                SentDate = DateTimeOffset.UtcNow.AddDays(-1),
                UserNotificationTypeId = (int)NotificationTypeEnum.UpcomingEventsInYourAreaToday,
            };

            var userNotifications = new List<UserNotification>() { userNotification };

            userNotificationRepository.Setup(ea => ea.GetUserNotifications(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(userNotifications);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasNotAlreadyReceivedTheEmail_Sends1Email()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            // The user has already received notifications for all events
            var userNotification = new UserNotification()
            {
                EventId = events[0].Id,
                UserId = users[0].Id,
                SentDate = DateTimeOffset.UtcNow.AddDays(-1),
                UserNotificationTypeId = (int)NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek,
            };

            var userNotifications = new List<UserNotification>() { userNotification };

            userNotificationRepository.Setup(ea => ea.GetUserNotifications(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(userNotifications);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Once);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasOptedOutOfAllEmails_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();

            users[0].IsOptedOutOfAllEmails = true;

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Never);
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Never);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Never);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventFor1UsersWhoHasOptedOutOfThisEmail_SendsNoEmail()
        {
            // Arrange
            List<Event> events = GetEventList1();
            List<User> users = GetUserList1();
            List<UserNotificationPreference> userNotificationPreferences = GetUserNotificationPreferences();
            var unIndex = userNotificationPreferences.FindIndex(unp => unp.UserNotificationTypeId == (int)NotificationType);
            userNotificationPreferences[unIndex].IsOptedOut = true;

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);
            userNotificationPreferenceRepository.Setup(unp => unp.GetUserNotificationPreferences(It.IsAny<Guid>())).ReturnsAsync(userNotificationPreferences);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, userNotificationPreferenceRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Never);
            userNotificationPreferenceRepository.Verify(_ => _.GetUserNotificationPreferences(It.IsAny<Guid>()), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Never);
            userNotificationRepository.Verify(_ => _.AddUserNotification(It.IsAny<UserNotification>()), Times.Never);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
