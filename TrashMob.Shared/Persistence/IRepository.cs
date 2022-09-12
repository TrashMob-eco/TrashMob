namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    /// <summary>
    /// Generic IRepository to cut down on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : BaseModel
    {
        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);
 
        Task<T> Add(T instance);

        Task<T> Update(T instance);

        Task<int> Delete(Guid id);

        Task<T> Get(Guid id, CancellationToken cancellationToken = default);
    }
}
