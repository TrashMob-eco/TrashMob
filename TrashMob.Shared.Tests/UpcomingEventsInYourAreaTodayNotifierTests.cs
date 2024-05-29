namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;

    public class UpcomingEventsInYourAreaTodayNotifierTests : UpcomingEventsInYourAreaNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaSoon;

        protected override INotificationEngine Engine => new UpcomingEventsInYourAreaSoonNotifier(EventManager.Object,
            UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object,
            NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object,
            Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 3;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 2;
    }
}