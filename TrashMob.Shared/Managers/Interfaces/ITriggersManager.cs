namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco.IFTTT;

    /// <summary>
    /// Defines operations for handling IFTTT trigger requests.
    /// </summary>
    public interface ITriggersManager
    {
        /// <summary>
        /// Gets event data for IFTTT trigger requests.
        /// </summary>
        /// <param name="triggerRequest">The IFTTT triggers request parameters.</param>
        /// <param name="userId">The unique identifier of the user making the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of IFTTT event responses matching the trigger criteria.</returns>
        public Task<List<IftttEventResponse>> GetEventsTriggerDataAsync(TriggersRequest triggerRequest, Guid userId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Validates an IFTTT trigger request.
        /// </summary>
        /// <param name="triggersRequest">The IFTTT triggers request to validate.</param>
        /// <param name="eventRequestType">The type of event request being validated.</param>
        /// <returns>The validation result object.</returns>
        public object ValidateRequest(TriggersRequest triggersRequest, EventRequestType eventRequestType);
    }
}
