namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner contacts.
    /// </summary>
    public interface IPartnerContactManager : IKeyedManager<PartnerContact>
    {
        /// <summary>
        /// Gets the partner associated with a specific contact.
        /// </summary>
        /// <param name="partnerContactId">The unique identifier of the partner contact.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The partner associated with the contact.</returns>
        Task<Partner> GetPartnerForContact(Guid partnerContactId, CancellationToken cancellationToken);
    }
}
