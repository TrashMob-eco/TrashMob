namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IBaseManager<T> where T : BaseModel
    {
        Task<T> Add(T instance);

        Task<T> Update(T instance);

        public Task<T> Add(T instance, Guid userId);

        public Task<T> Update(T instance, Guid userId);

        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);

        public Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken);

        public Task<IEnumerable<T>> GetByParentId(Guid parentId, CancellationToken cancellationToken);

        public Task<T> Get(Guid parentId, int secondId, CancellationToken cancellationToken);

        public Task<T> Get(Guid parentId, Guid secondId, CancellationToken cancellationToken);

        public Task Delete(Guid parentId, int secondId, CancellationToken cancellationToken);

        public Task Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken);
    }
}
