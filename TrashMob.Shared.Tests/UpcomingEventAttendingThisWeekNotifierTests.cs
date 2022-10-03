namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventAttendingThisWeekNotifierTests : UpcomingEventAttendingNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingThisWeek;

        protected override INotificationEngine Engine => new UpcomingEventAttendingThisWeekNotifier(EventRepository.Object, UserManager.Object, EventAttendeeRepository.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 8;
    }
}
