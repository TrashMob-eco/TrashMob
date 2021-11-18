namespace TrashMobMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IEventTypeRestService
    {
        Task<IEnumerable<EventType>> GetEventTypesAsync();
    }
}