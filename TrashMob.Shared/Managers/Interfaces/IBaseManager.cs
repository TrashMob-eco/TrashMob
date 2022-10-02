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
        Task<T> Add(T instance);

        Task<T> Update(T instance);

        Task<T> Add(T instance, Guid userId);

        Task<T> Update(T instance, Guid userId);

        Task<IEnumerable<T>> Get(CancellationToken cancellationToken);

        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetByParentId(Guid parentId, CancellationToken cancellationToken);

        Task<T> Get(Guid parentId, int secondId, CancellationToken cancellationToken);

        Task<T> Get(Guid parentId, Guid secondId, CancellationToken cancellationToken);

        Task Delete(Guid parentId, int secondId, CancellationToken cancellationToken);

        Task Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken);
    }
}
