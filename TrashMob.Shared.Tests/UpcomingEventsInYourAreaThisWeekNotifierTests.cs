namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventsInYourAreaThisWeekNotifierTests : UpcomingEventsInYourAreaNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override INotificationEngine Engine => new UpcomingEventsInYourAreaThisWeekNotifier(EventRepository.Object, UserRepository.Object, EventAttendeeRepository.Object, UserNotificationRepository.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 8;
    }
}
