namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IBaseManager<T> where T : BaseModel
    {
        Task<T> Add(T instance);

        Task<T> Update(T instance);

        public Task<T> Add(T instance, Guid userId);

        public Task<T> Update(T instance, Guid userId);

        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);

        public Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken);
    }
}
