namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventAttendingTodayNotifierTests :  UpcomingEventAttendingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingSoon;

        protected override INotificationEngine Engine => new UpcomingEventAttendingSoonNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, UserNotificationPreferenceRepository.Object, EmailSender.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 2;

        [Fact]
        public void GetHtmlEmailTemplate_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventAttendingSoonNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, UserNotificationPreferenceRepository.Object, EmailSender.Object, MapRepository.Object, Logger.Object);

            // Act
            var template = engine.GetHtmlEmailTemplate();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(template));
            Assert.Contains("You're attending a TrashMob.eco event today!", template);
        }
    }
}
