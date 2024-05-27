namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco.IFTTT;

    public interface ITriggersManager
    {
        public Task<List<IftttEventResponse>> GetEventsTriggerDataAsync(TriggersRequest triggerRequest, Guid userId,
            CancellationToken cancellationToken);

        public object ValidateRequest(TriggersRequest triggersRequest, EventRequestType eventRequestType);
    }
}