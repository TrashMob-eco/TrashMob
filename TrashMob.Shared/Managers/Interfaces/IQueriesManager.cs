namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco.IFTTT;

    public interface IQueriesManager
    {
        public Task<List<IftttEventResponse>> GetEventsQueryDataAsync(QueriesRequest queriesRequest, Guid userId, CancellationToken cancellationToken);
    }
}
