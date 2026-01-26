namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager class for entities with a GUID primary key, extending BaseManager with key-based operations.
    /// </summary>
    /// <typeparam name="T">The entity type derived from KeyedModel.</typeparam>
    public class KeyedManager<T> : BaseManager<T>, IKeyedManager<T> where T : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedManager{T}"/> class.
        /// </summary>
        /// <param name="repository">The keyed repository for data access operations.</param>
        public KeyedManager(IKeyedRepository<T> repository) : base(repository)
        {
        }

        /// <summary>
        /// Gets the repository cast to the keyed repository interface.
        /// </summary>
        protected IKeyedRepository<T> Repo => Repository as IKeyedRepository<T>;

        /// <inheritdoc />
        public override Task<T> AddAsync(T instance, CancellationToken cancellationToken = default)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            return base.AddAsync(instance, cancellationToken);
        }

        /// <inheritdoc />
        public override Task<T> AddAsync(T instance, Guid userId, CancellationToken cancellationToken = default)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            return base.AddAsync(instance, userId, cancellationToken);
        }

        /// <inheritdoc />
        public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Repo.DeleteAsync(id);
        }

        /// <inheritdoc />
        public virtual Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Repo.GetAsync(id, cancellationToken);
        }
    }
}