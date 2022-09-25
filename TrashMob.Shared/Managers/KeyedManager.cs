
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class KeyedManager<T> : BaseManager<T>, IKeyedManager<T> where T : KeyedModel
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

        public override Task<T> Add(T instance)
        {
            instance.Id = Guid.NewGuid();
            return Repo.Add(instance);
        }

        public virtual Task<int> Delete(Guid id)
        {
            return Repo.Delete(id);
        }

        public virtual Task<T> Get(Guid id, CancellationToken cancellationToken = default)
        {
            return Repo.Get(id, cancellationToken);
        }
    }
}
