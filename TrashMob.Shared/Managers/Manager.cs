
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public abstract class Manager<T> : IManager<T> where T : BaseModel
    {
        public Manager(IRepository<T> repository)
        {
            Repository = repository;
        }

        protected IRepository<T> Repository { get; }

        public virtual Task<T> Add(T instance)
        {
            instance.Id = Guid.NewGuid();
            return Repository.Add(instance);
        }

        public virtual Task<int> Delete(Guid id)
        {
            return Repository.Delete(id);
        }

        public virtual IQueryable<T> Get()
        {
            return Repository.Get();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return Repository.Get(expression);
        }

        public virtual Task<T> Get(Guid id, CancellationToken cancellationToken = default)
        {
            return Repository.Get(id, cancellationToken);
        }

        public virtual Task<T> Update(T instance)
        {
            return Repository.Update(instance);
        }
    }
}
