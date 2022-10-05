
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

    public class LookupManager<T> : ILookupManager<T> where T : LookupModel
    {
        public LookupManager(ILookupRepository<T> repository)
        {
            Repository = repository;
        }

        protected ILookupRepository<T> Repository { get; }

        public virtual async Task<IEnumerable<T>> Get()
        {
            return (await Repository.Get().ToListAsync()).AsEnumerable();
        }

        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> expression)
        {
            return (await Repository.Get(expression).ToListAsync()).AsEnumerable();
        }

        public virtual Task<T> Get(int id, CancellationToken cancellationToken = default)
        {
            return Repository.Get(id, cancellationToken);
        }
    }
}
