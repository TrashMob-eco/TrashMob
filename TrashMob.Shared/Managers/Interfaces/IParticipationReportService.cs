namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Generates and sends official participation report emails with PDF attachments.
    /// </summary>
    public interface IParticipationReportService
    {
        /// <summary>
        /// Sends a participation report email to a specific attendee for an event.
        /// Only sends if the attendee has Approved or Adjusted metrics.
        /// </summary>
        Task<ServiceResult<bool>> SendReportAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Sends participation report emails to all attendees with Approved or Adjusted metrics.
        /// Must be called by an event lead.
        /// </summary>
        Task<ServiceResult<int>> SendAllReportsAsync(Guid eventId, Guid requestingUserId, CancellationToken cancellationToken);
    }
}
