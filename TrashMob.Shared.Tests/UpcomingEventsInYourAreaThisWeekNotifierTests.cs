namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventsInYourAreaThisWeekNotifierTests : UpcomingEventsInYourAreaNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override INotificationEngine Engine => new UpcomingEventsInYourAreaThisWeekNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, UserNotificationPreferenceRepository.Object, EmailSender.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 8;

        [Fact]
        public void GetHtmlEmailTemplate_Succeeds()
        {
            // Arrange
            var engine = new UpcomingEventsInYourAreaThisWeekNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, UserNotificationPreferenceRepository.Object, EmailSender.Object, MapRepository.Object, Logger.Object);

            // Act
            var template = engine.GetHtmlEmailTemplate();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(template));
            Assert.Contains("Upcoming TrashMob.eco events in your area this week!", template);
        }
    }
}
