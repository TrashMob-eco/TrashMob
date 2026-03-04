namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class DependentManager(
        IKeyedRepository<Dependent> repository,
        IKeyedRepository<User> userRepository)
        : KeyedManager<Dependent>(repository), IDependentManager
    {
        public async Task<IEnumerable<Dependent>> GetByParentUserIdAsync(
            Guid parentUserId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(d => d.ParentUserId == parentUserId && d.IsActive)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync(cancellationToken);
        }

        public override async Task<Dependent> AddAsync(Dependent instance, Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetAsync(instance.ParentUserId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("Parent user not found.");
            }

            instance.IsActive = true;
            return await base.AddAsync(instance, userId, cancellationToken);
        }

        public async Task<int> SoftDeleteAsync(Guid dependentId, Guid userId, CancellationToken cancellationToken = default)
        {
            var dependent = await Repo.GetAsync(dependentId, cancellationToken);
            if (dependent == null)
            {
                return 0;
            }

            dependent.IsActive = false;
            await UpdateAsync(dependent, userId, cancellationToken);
            return 1;
        }
    }
}
