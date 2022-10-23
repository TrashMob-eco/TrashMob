namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerSocialMediaAccountManager : IKeyedManager<PartnerSocialMediaAccount>
    {
        Task<Partner> GetPartnerForSocialMediaAccount(Guid partnerSocialMediaAccountId, CancellationToken cancellationToken);
    }
}
