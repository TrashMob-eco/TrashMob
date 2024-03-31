namespace TrashMobMobile.Data
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface ILitterImageRestService
    {
        Task<IEnumerable<LitterImage>> GetLitterImagesAsync(Guid litterReportId, CancellationToken cancellationToken = default);

        Task<LitterImage> GetLitterImageAsync(Guid litterImageId, CancellationToken cancellationToken = default);

        Task<string> GetLitterImageFileAsync(Guid litterImageId, CancellationToken cancellationToken = default);

        Task<LitterImage> AddLitterImageAsync(LitterImage litterImage, CancellationToken cancellationToken = default);

        Task AddLitterImageFileAsync(Guid litterReportId, Guid litterImageId, string localFileName, CancellationToken cancellationToken = default);

        Task DeleteLitterImageAsync(Guid litterImageId, CancellationToken cancellationToken = default);

        Task<LitterImage> UpdateLitterImageAsync(LitterImage litterImage, CancellationToken cancellationToken = default);
    }
}