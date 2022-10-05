namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Generic IRepository to cut down on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> where T : BaseModel
    {
        Task<T> Add(T instance);

        Task<T> Update(T instance);

        IQueryable<T> Get();

        IQueryable<T> Get(Expression<Func<T, bool>> expression);

        Task<int> Delete(T instance);
    }
}
