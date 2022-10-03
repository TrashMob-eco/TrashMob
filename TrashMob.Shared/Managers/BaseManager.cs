
namespace TrashMob.Shared.Managers
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class BaseManager<T> : IBaseManager<T> where T : BaseModel
    {
        public BaseManager(IBaseRepository<T> repository)
        {
            Repository = repository;
        }

        protected virtual IBaseRepository<T> Repository { get; }

        public virtual Task<T> Add(T instance, Guid userId)
        {
            instance.CreatedByUserId = userId;
            instance.CreatedDate = DateTimeOffset.UtcNow;
            instance.LastUpdatedByUserId = userId;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.Add(instance);
        }

        public virtual Task<T> Update(T instance, Guid userId)
        {
            instance.LastUpdatedByUserId = userId;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.Update(instance);
        }

        public virtual Task<T> Add(T instance)
        {
            return Repository.Add(instance);
        }

        public virtual Task<T> Update(T instance)
        {
            return Repository.Update(instance);
        }

        public virtual async Task<IEnumerable<T>> Get(CancellationToken cancellationToken)
        {
            return await Repository.Get().ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> expression, CancellationToken cancellationToken)
        {
            return await Repository.Get(expression).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var results = await Repository.Get()
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);

            return results;
        }

        public virtual Task<IEnumerable<T>> GetByParentId(Guid parentId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> Get(Guid parentId, int secondId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> Get(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task Delete(Guid parentId, int secondId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<T>> GetCollection(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
