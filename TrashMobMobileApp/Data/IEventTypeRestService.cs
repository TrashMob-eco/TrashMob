namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IEventTypeRestService
    {
        Task<IEnumerable<EventType>> GetEventTypesAsync();
    }
}