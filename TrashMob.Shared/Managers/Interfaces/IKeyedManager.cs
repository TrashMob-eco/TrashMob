namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IKeyedManager<T> : IBaseManager<T> where T : KeyedModel
    {
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default);
    }
}