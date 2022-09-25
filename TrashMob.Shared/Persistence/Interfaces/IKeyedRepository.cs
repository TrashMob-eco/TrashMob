namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Generic IRepository to cut down on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IKeyedRepository<T> : IBaseRepository<T> where T : KeyedModel
    {
        Task<int> Delete(Guid id);

        Task<T> Get(Guid id, CancellationToken cancellationToken = default);
    }
}
