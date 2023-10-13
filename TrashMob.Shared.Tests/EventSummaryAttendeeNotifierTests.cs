namespace TrashMob.Shared.Tests
{
    using Moq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class EventSummaryAttendeeNotifierTests : NotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryAttendee;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 4 * 24;

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            // Arrange
            var engine = new EventSummaryAttendeeNotifier(EventManager.Object, UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

            // Act
            await engine.GenerateNotificationsAsync().ConfigureAwait(false);

            // Assert
            EmailManager.Verify(_ => _.SendTemplatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
