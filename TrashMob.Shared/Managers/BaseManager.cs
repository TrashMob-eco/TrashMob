
namespace TrashMob.Shared.Managers
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class BaseManager<T> : IBaseManager<T> where T : BaseModel
    {
        public BaseManager(IBaseRepository<T> repository)
        {
            Repository = repository;
        }

        protected IBaseRepository<T> Repository { get; }

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

        public virtual IQueryable<T> Get()
        {
            return Repository.Get();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return Repository.Get(expression);
        }

        public virtual async Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var results = await Repository.Get()
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
