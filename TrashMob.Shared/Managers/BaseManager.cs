namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Abstract base manager class providing common CRUD operations for entities derived from BaseModel.
    /// </summary>
    /// <typeparam name="T">The entity type derived from BaseModel.</typeparam>
    public abstract class BaseManager<T>(IBaseRepository<T> repository)
        : IBaseManager<T> where T : BaseModel
    {
        /// <summary>
        /// Gets the repository used for data access operations.
        /// </summary>
        protected virtual IBaseRepository<T> Repository { get; } = repository;

        /// <inheritdoc />
        public virtual Task<T> AddAsync(T instance, Guid userId, CancellationToken cancellationToken = default)
        {
            instance.CreatedByUserId = userId;
            instance.CreatedDate = DateTimeOffset.UtcNow;
            instance.LastUpdatedByUserId = userId;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.AddAsync(instance);
        }

        /// <inheritdoc />
        public virtual Task<T> UpdateAsync(T instance, Guid userId, CancellationToken cancellationToken = default)
        {
            instance.LastUpdatedByUserId = userId;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.UpdateAsync(instance);
        }

        /// <inheritdoc />
        public virtual Task<T> AddAsync(T instance, CancellationToken cancellationToken = default)
        {
            instance.CreatedDate = DateTimeOffset.UtcNow;
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.AddAsync(instance);
        }

        /// <inheritdoc />
        public virtual Task<T> UpdateAsync(T instance, CancellationToken cancellationToken = default)
        {
            instance.LastUpdatedDate = DateTimeOffset.UtcNow;
            return Repository.UpdateAsync(instance);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.Get().ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            return await Repository.Get(expression).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetByCreatedUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var results = await Repository.Get()
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);

            return results;
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<T>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<T> GetAsync(Guid parentId, int secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<T> GetAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<int> DeleteAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<int> Delete(Guid parentId, int secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<T>> GetCollectionAsync(Guid parentId, Guid secondId,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}