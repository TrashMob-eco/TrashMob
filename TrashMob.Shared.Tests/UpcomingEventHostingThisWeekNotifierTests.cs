namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventHostingThisWeekNotifierTests : UpcomingEventHostingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingThisWeek;

        protected override INotificationEngine Engine => new UpcomingEventHostingThisWeekNotifier(EventManager.Object, UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 8;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 3 * 24;
    }
}
