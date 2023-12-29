namespace TrashMobMobile.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    public interface IStatsRestService
    {
        Task<Stats> GetStatsAsync(CancellationToken cancellationToken = default);

        Task<Stats> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}