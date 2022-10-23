
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

        public virtual Task<T> AddAsync(T instance, Guid userId, CancellationToken cancellationToken = default)
        {
            instance.CreatedByUserId = userId;
            instance.CreatedDate = DateTimeOffset.UtcNow;
            instance.LastUpdatedByUserId = userId;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.AddAsync(instance);
        }

        public virtual Task<T> UpdateAsync(T instance, Guid userId, CancellationToken cancellationToken = default)
        {
            instance.LastUpdatedByUserId = userId;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.UpdateAsync(instance);
        }

        public virtual Task<T> AddAsync(T instance, CancellationToken cancellationToken = default)
        {
            instance.CreatedDate = DateTimeOffset.UtcNow;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.AddAsync(instance);
        }

        public virtual Task<T> UpdateAsync(T instance, CancellationToken cancellationToken = default)
        {
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.UpdateAsync(instance);
        }

        public virtual async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.Get().ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(expression).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var results = await Repository.Get()
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);

            return results;
        }

        public virtual Task<IEnumerable<T>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> GetAsync(Guid parentId, int secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> GetAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> DeleteAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> Delete(Guid parentId, int secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<T>> GetCollectionAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
