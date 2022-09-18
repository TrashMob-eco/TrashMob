
namespace TrashMob.Shared.Managers
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public abstract class ExtendedManager<T> : Manager<T>, IExtendedManager<T> where T : ExtendedBaseModel
    {
        public ExtendedManager(IRepository<T> repository) : base(repository)
        {
        }

        public virtual Task<T> Add(T instance, Guid userId)
        {
            instance.Id = Guid.NewGuid();
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

        public virtual async Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var results = await Repository.Get()
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
