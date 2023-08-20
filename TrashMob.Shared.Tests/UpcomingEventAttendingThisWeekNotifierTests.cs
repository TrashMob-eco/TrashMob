namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventAttendingThisWeekNotifierTests : UpcomingEventAttendingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingThisWeek;

        protected override INotificationEngine Engine => new UpcomingEventAttendingThisWeekNotifier(EventManager.Object, UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventMaxOutOfWindow => 8;

        protected override int NumberOfHoursToAddForEventMinOutOfWindow => 3*24;
    }
}
