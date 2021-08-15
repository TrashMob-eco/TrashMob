namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
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

        public async Task<IEnumerable<MediaType>> GetAllMediaTypes()
        {
            return await mobDbContext.MediaTypes
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
