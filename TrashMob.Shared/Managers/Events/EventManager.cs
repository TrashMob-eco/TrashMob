namespace TrashMob.Shared.Managers.Events
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventManager : KeyedManager<Event>, IKeyedManager<Event>
    {
        private readonly IEmailManager emailManager;

        public EventManager(IKeyedRepository<Event> repository, IEmailManager emailManager) : base(repository)
        {
            this.emailManager = emailManager;
        }
    }
}
