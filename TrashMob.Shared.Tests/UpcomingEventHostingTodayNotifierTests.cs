namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;

    public class UpcomingEventHostingTodayNotifierTests : UpcomingEventHostingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingSoon;

        protected override INotificationEngine Engine => new UpcomingEventHostingSoonNotifier(EventManager.Object,
            UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object,
            NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object,
            Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 2;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 2;
    }
}