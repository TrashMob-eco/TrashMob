namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    /// <summary>
    /// Generic ILookupRepository to cut down on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILookupRepository<T> where T : LookupModel
    {
        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);
 
        Task<T> Get(int id, CancellationToken cancellationToken = default);
    }
}
