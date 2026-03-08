namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Linq;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Defines operations for managing partners.
    /// </summary>
    public interface IPartnerManager : IKeyedManager<Partner>
    {
        /// <summary>
        /// Gets a queryable of filtered active partners for V2 API pagination. The returned IQueryable
        /// is not materialized, allowing the caller to apply ToPagedAsync() for database-side pagination.
        /// </summary>
        /// <param name="filter">The V2 query parameters with partner-specific filters.</param>
        /// <returns>An unmaterialized queryable of active partners matching the filter.</returns>
        IQueryable<Partner> GetFilteredPartnersQueryable(PartnerQueryParameters filter);
    }
}
