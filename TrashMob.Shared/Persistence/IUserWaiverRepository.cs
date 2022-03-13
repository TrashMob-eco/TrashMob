namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IUserWaiverRepository
    {
        IQueryable<UserWaiver> GetUserWaivers(CancellationToken cancellationToken = default);

        Task<UserWaiver> AddUserWaiver(UserWaiver userWaiver);

        Task<UserWaiver> UpdateUserWaiver(UserWaiver userWaiver);

        Task<int> DeleteUserWaiver(Guid partnerId, Guid userId);
    }
}
