namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner social media accounts.
    /// </summary>
    public interface IPartnerSocialMediaAccountManager : IKeyedManager<PartnerSocialMediaAccount>
    {
        /// <summary>
        /// Gets the partner associated with a specific social media account.
        /// </summary>
        /// <param name="partnerSocialMediaAccountId">The unique identifier of the partner social media account.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The partner associated with the social media account.</returns>
        Task<Partner> GetPartnerForSocialMediaAccount(Guid partnerSocialMediaAccountId,
            CancellationToken cancellationToken);
    }
}
