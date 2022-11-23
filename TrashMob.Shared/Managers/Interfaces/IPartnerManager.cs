namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerManager : IKeyedManager<Partner>
    {
        Task CreatePartnerAsync(PartnerRequest partnerRequest);
    }
}
