namespace TrashMob.Shared.Managers
{
    using System.Linq.Expressions;
    using System.Linq;
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IManager<T> where T : BaseModel
    {
        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);

        Task<T> Add(T instance);

        Task<T> Update(T instance);

        Task<int> Delete(Guid id);

        Task<T> Get(Guid id, CancellationToken cancellationToken = default);
    }
}
