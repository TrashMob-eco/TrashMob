namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing associations between events and litter reports.
    /// </summary>
    public interface IEventLitterReportManager : IBaseManager<EventLitterReport>
    {
        /// <summary>
        /// Gets all events that a litter report is associated with.
        /// </summary>
        /// <param name="litterReportId">The unique identifier of the litter report.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events associated with the specified litter report.</returns>
        Task<IEnumerable<Event>> GetEventsLitterReportIsAssociatedToAsync(Guid litterReportId, CancellationToken cancellationToken = default);
    }
}
