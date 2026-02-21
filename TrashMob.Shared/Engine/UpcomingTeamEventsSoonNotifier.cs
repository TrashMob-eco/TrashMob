namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that notifies team members about team-only events happening within the next 24 hours.
    /// </summary>
    public class UpcomingTeamEventsSoonNotifier(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        ITeamMemberManager teamMemberManager,
        ILogger logger)
        : UpcomingTeamEventsBaseNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager,
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, teamMemberManager, logger),
            INotificationEngine
    {

        /// <inheritdoc />
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingTeamEventsSoon;

        /// <inheritdoc />
        protected override string EmailSubject => "Upcoming team event on TrashMob.eco soon!";

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 1;
    }
}
