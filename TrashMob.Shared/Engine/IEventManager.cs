namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;

    public interface IEventManager
    {
        public Task GetUpcomingEventsAsync();
    }
}
