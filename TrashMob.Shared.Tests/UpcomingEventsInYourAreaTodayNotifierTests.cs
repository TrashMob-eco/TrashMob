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

    public class UpcomingEventsInYourAreaTodayNotifierTests
    {
        private readonly Mock<IEventRepository> eventRepository;
        private readonly Mock<IEventAttendeeRepository> eventAttendeeRepository;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IUserNotificationRepository> userNotificationRepository;
        private readonly Mock<IEmailSender> emailSender;

        public UpcomingEventsInYourAreaTodayNotifierTests()
        {
            eventRepository = new Mock<IEventRepository>();
            eventAttendeeRepository = new Mock<IEventAttendeeRepository>();
            userRepository = new Mock<IUserRepository>();
            userNotificationRepository = new Mock<IUserNotificationRepository>();
            emailSender = new Mock<IEmailSender>();
        }

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void GetEmailTemplate_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, emailSender.Object);

            // Act
            var template = engine.GetEmailTemplate();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(template));
            Assert.Contains("There are upcoming TrashMob.eco events in your area today!", template);
        }

        [Fact]
        public async Task GenerateNotificationsAsync_With1EventForUser_Sends1Email()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var createdById = Guid.NewGuid();

            var relevantEvent = new Event()
            {
                ActualNumberOfParticipants = 0,
                City = "Seattle",
                Country = "United States",
                CreatedByUserId = createdById,
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-14),
                Description = "This is a test event",
                EventDate = DateTimeOffset.UtcNow.AddMinutes(10),
                EventStatusId = (int)EventStatusEnum.Active,
                EventTypeId = 3,
                Id = Guid.NewGuid(),
                LastUpdatedByUserId = createdById,
                LastUpdatedDate = DateTimeOffset.UtcNow.AddDays(-13),
                Latitude = 50,
                Longitude = 45,
                MaxNumberOfParticipants = 10,
                Name = "Test Event",
                PostalCode = "98040",
                Region = "Washington",
                StreetAddress = "1 King Street"
            };

            var user = new User()
            {
                City = "Seattle",
                Country = "United States",
                DateAgreedToPrivacyPolicy = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTermsOfService = DateTimeOffset.UtcNow.AddDays(-2),
                Email = "testuser@trashmob.eco",
                GivenName = "Test",
                Id = currentUserId,
                MemberSince = DateTimeOffset.UtcNow.AddDays(-2),
                NameIdentifier = "123456789",
                PostalCode = "98040",
                PrivacyPolicyVersion = "1.0",
                Region = "Washington",
                SourceSystemUserName = "TestUser",
                SurName = "Bleg",
                TermsOfServiceVersion = "1.0",
                UserName = "BlegD",
            };

            var events = new List<Event>
            {
                relevantEvent
            };

            var users = new List<User>
            { 
                user
            };

            eventRepository.Setup(e => e.GetActiveEvents()).ReturnsAsync(events);
            userRepository.Setup(u => u.GetAllUsers()).ReturnsAsync(users);

            var engine = new UpcomingEventsInYourAreaTodayNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            eventRepository.Verify(_ => _.GetActiveEvents(), Times.Once);
            userRepository.Verify(_ => _.GetAllUsers(), Times.Once);
            eventAttendeeRepository.Verify(_ => _.GetEventsUserIsAttending(It.IsAny<Guid>()), Times.Once);
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
