namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventHostingTodayNotifierTests : UpcomingEventHostingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingSoon;

        protected override INotificationEngine Engine => new UpcomingEventHostingSoonNotifier(EventRepository.Object, UserManager.Object, EventAttendeeRepository.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 2;
    }
}
