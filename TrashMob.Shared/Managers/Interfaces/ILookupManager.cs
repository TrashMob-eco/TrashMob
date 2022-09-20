namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Linq.Expressions;
    using System.Linq;
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface ILookupManager<T> where T : LookupModel
    {
        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);

        Task<T> Get(int id, CancellationToken cancellationToken = default);
    }
}
