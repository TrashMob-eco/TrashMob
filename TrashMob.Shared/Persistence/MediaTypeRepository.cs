namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Models;

    public class MediaTypeRepository : IMediaTypeRepository
    {
        private readonly MobDbContext mobDbContext;

        public MediaTypeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<MediaType>> GetAllMediaTypes(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.MediaTypes.Where(e => e.IsActive == true)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
