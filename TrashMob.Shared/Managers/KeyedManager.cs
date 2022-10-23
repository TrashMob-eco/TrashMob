
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class KeyedManager<T> : BaseManager<T>, IKeyedManager<T> where T : KeyedModel
    {
        public KeyedManager(IKeyedRepository<T> repository) : base(repository)
        {

        }

        protected IKeyedRepository<T> Repo
        {
            get
            {
                return Repository as IKeyedRepository<T>;
            }
        }

        public override Task<T> AddAsync(T instance, CancellationToken cancellationToken = default)
        {
            instance.Id = Guid.NewGuid();
            return base.AddAsync(instance, cancellationToken);
        }

        public override Task<T> AddAsync(T instance, Guid userId, CancellationToken cancellationToken = default)
        {
            instance.Id = Guid.NewGuid();
            return base.AddAsync(instance, userId, cancellationToken);
        }

        public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Repo.DeleteAsync(id);
        }

        public virtual Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Repo.GetAsync(id, cancellationToken);
        }
    }
}
