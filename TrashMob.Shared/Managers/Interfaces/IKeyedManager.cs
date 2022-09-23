namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IKeyedManager<T> : IBaseManager<T> where T : KeyedModel
    {
        Task<T> Get(Guid id, CancellationToken cancellationToken = default);

        Task<int> Delete(Guid id);
    }
}
