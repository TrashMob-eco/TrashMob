namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IWaiverManager : IKeyedManager<Waiver>
    {
        Task<Waiver> GetByNameAsync(string waiverName, CancellationToken cancellationToken = default);
    }
}