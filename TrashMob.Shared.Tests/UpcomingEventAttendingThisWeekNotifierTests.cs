namespace TrashMob.Shared.Tests
{
    using Moq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Persistence;
    using Xunit;

    public class UpcomingEventAttendingThisWeekNotifierTests
    {
        private readonly Mock<IEventRepository> eventRepository;
        private readonly Mock<IEventAttendeeRepository> eventAttendeeRepository;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IUserNotificationRepository> userNotificationRepository;
        private readonly Mock<IEmailSender> emailSender;

        public UpcomingEventAttendingThisWeekNotifierTests()
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
            var engine = new UpcomingEventAttendingThisWeekNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, emailSender.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            emailSender.Verify(_ => _.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void GetEmailTemplate_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventAttendingThisWeekNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, userNotificationRepository.Object, emailSender.Object);

            // Act
            var template = engine.GetEmailTemplate();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(template));
            Assert.Contains("You're attending a TrashMob.eco event this week!", template);
        }
    }
}
