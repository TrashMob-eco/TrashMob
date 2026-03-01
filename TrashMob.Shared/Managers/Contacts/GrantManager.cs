namespace TrashMob.Shared.Managers.Contacts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class GrantManager(IKeyedRepository<Grant> repository)
        : KeyedManager<Grant>(repository), IGrantManager
    {
        public async Task<IEnumerable<Grant>> GetByStatusAsync(int status, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(g => g.Status == status)
                .OrderBy(g => g.SubmissionDeadline)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
