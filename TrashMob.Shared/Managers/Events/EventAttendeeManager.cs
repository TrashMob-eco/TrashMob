namespace TrashMob.Shared.Managers.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using System;
    using Microsoft.EntityFrameworkCore;

    public class EventAttendeeManager : BaseManager<EventAttendee>, IBaseManager<EventAttendee>
    {
        private readonly IEmailManager emailManager;

        public EventAttendeeManager(IBaseRepository<EventAttendee> repository, IEmailManager emailManager) : base(repository)
        {
            this.emailManager = emailManager;
        }
    }
}
