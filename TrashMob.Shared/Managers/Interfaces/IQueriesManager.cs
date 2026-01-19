namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco.IFTTT;

    /// <summary>
    /// Defines operations for handling IFTTT query requests.
    /// </summary>
    public interface IQueriesManager
    {
        /// <summary>
        /// Gets event data for IFTTT query requests.
        /// </summary>
        /// <param name="queriesRequest">The IFTTT queries request parameters.</param>
        /// <param name="userId">The unique identifier of the user making the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of IFTTT event responses matching the query.</returns>
        public Task<List<IftttEventResponse>> GetEventsQueryDataAsync(QueriesRequest queriesRequest, Guid userId,
            CancellationToken cancellationToken);
    }
}
