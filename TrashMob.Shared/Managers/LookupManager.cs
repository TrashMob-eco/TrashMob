
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class LookupManager<T> : ILookupManager<T> where T : LookupModel
    {
        public LookupManager(ILookupRepository<T> repository)
        {
            Repository = repository;
        }

        protected ILookupRepository<T> Repository { get; }

        public virtual IQueryable<T> Get()
        {
            return Repository.Get();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return Repository.Get(expression);
        }

        public virtual Task<T> Get(int id, CancellationToken cancellationToken = default)
        {
            return Repository.Get(id, cancellationToken);
        }
    }
}
