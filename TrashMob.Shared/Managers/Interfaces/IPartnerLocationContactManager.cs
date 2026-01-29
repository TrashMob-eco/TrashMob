namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner location contacts.
    /// </summary>
    public interface IPartnerLocationContactManager : IKeyedManager<PartnerLocationContact>
    {
        /// <summary>
        /// Gets the partner associated with a specific location contact.
        /// </summary>
        /// <param name="partnerLocationContactId">The unique identifier of the partner location contact.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The partner associated with the location contact.</returns>
        Task<Partner> GetPartnerForLocationContact(Guid partnerLocationContactId, CancellationToken cancellationToken);
    }
}
