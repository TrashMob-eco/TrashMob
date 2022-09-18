
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IExtendedManager<T> : IManager<T> where T : ExtendedBaseModel
    {
        public Task<T> Add(T instance, Guid userId);

        public Task<T> Update(T instance, Guid userId);

        public Task<IEnumerable<T>> GetByUserId(Guid userId, CancellationToken cancellationToken);
    }
}
