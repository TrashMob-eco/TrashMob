namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Linq.Expressions;
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using System.Collections.Generic;

    public interface ILookupManager<T> where T : LookupModel
    {
        Task<IEnumerable<T>> Get();

        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> expression);

        Task<T> Get(int id, CancellationToken cancellationToken = default);
    }
}
