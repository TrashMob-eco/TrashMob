namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that notifies team members about team-only events happening within the next week (2-7 days).
    /// </summary>
    public class UpcomingTeamEventsThisWeekNotifier(
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
        protected override NotificationTypeEnum NotificationType =>
            NotificationTypeEnum.UpcomingTeamEventsThisWeek;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 7 * 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 2 * 24;

        /// <inheritdoc />
        protected override string EmailSubject => "Upcoming team event on TrashMob.eco this week!";
    }
}
