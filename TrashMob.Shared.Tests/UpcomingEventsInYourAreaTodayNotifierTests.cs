namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventsInYourAreaTodayNotifierTests : UpcomingEventsInYourAreaNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaSoon;

        protected override INotificationEngine Engine => new UpcomingEventsInYourAreaSoonNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, UserNotificationPreferenceRepository.Object, EmailSender.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 3;

        [Fact]
        public void GetHtmlEmailTemplate_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventsInYourAreaSoonNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, UserNotificationPreferenceRepository.Object, EmailSender.Object, MapRepository.Object, Logger.Object);

            // Act
            var template = engine.GetHtmlEmailTemplate();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(template));
            Assert.Contains("Upcoming TrashMob.eco events in your area today!", template);
        }
    }
}
