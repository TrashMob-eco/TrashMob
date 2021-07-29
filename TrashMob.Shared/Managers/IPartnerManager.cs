namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerManager
    {
        Task CreatePartner(PartnerRequest partnerRequest);
    }
}
