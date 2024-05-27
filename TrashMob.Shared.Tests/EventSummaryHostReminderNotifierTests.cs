namespace TrashMob.Shared.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class EventSummaryHostReminderNotifierTests : NotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryHostReminder;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 2;

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            // Arrange
            var engine = new EventSummaryHostReminderNotifier(EventManager.Object, UserManager.Object,
                EventAttendeeManager.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object,
                EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

            // Act
            await engine.GenerateNotificationsAsync();

            // Assert
            EmailManager.Verify(
                _ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}