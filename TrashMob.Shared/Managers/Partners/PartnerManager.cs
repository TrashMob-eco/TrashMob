namespace TrashMob.Shared.Managers.Partners
{
    using System.Linq;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for partner operations.
    /// </summary>
    public class PartnerManager(IKeyedRepository<Partner> repository)
        : KeyedManager<Partner>(repository), IPartnerManager
    {
        /// <inheritdoc />
        public IQueryable<Partner> GetFilteredPartnersQueryable(PartnerQueryParameters filter)
        {
            var query = Repo.Get()
                .Where(p => p.PartnerStatusId == (int)PartnerStatusEnum.Active);

            if (filter.PartnerTypeId != null)
            {
                query = query.Where(p => p.PartnerTypeId == filter.PartnerTypeId.Value);
            }

            if (filter.City != null)
            {
                query = query.Where(p => p.City == filter.City);
            }

            if (filter.Region != null)
            {
                query = query.Where(p => p.Region == filter.Region);
            }

            if (filter.Country != null)
            {
                query = query.Where(p => p.Country == filter.Country);
            }

            if (filter.HomePageEnabled != null)
            {
                query = query.Where(p => p.HomePageEnabled == filter.HomePageEnabled.Value);
            }

            if (filter.IsFeatured != null)
            {
                query = query.Where(p => p.IsFeatured == filter.IsFeatured.Value);
            }

            return query.OrderBy(p => p.Name);
        }
    }
}
