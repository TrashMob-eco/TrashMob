namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IMediaTypeRepository
    {
        Task<IEnumerable<MediaType>> GetAllMediaTypes(CancellationToken cancellationToken = default);
    }
}
