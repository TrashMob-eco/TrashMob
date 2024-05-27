namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;

    public class UpcomingEventAttendingTodayNotifierTests : UpcomingEventAttendingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingSoon;

        protected override INotificationEngine Engine => new UpcomingEventAttendingSoonNotifier(EventManager.Object,
            UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object,
            NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object,
            Logger.Object);

        protected override int NumberOfDaysToAddForEventMaxOutOfWindow => 2;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 2;
    }
}