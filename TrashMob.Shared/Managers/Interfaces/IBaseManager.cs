namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IBaseManager<T> where T : BaseModel
    {
        Task<T> AddAsync(T instance, CancellationToken cancellationToken);

        Task<T> UpdateAsync(T instance, CancellationToken cancellationToken);

        Task<T> AddAsync(T instance, Guid userId, CancellationToken cancellationToken);

        Task<T> UpdateAsync(T instance, Guid userId, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken);

        Task<T> GetAsync(Guid parentId, int secondId, CancellationToken cancellationToken);

        Task<T> GetAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetCollectionAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken);
        
        Task<int> DeleteAsync(Guid parentId, CancellationToken cancellationToken);

        Task<int> Delete(Guid parentId, int secondId, CancellationToken cancellationToken);

        Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken);
    }
}
