namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner documents.
    /// </summary>
    public interface IPartnerDocumentManager : IKeyedManager<PartnerDocument>
    {
        /// <summary>
        /// Gets the partner associated with a specific document.
        /// </summary>
        /// <param name="partnerDocumentId">The unique identifier of the partner document.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The partner associated with the document.</returns>
        Task<Partner> GetPartnerForDocument(Guid partnerDocumentId, CancellationToken cancellationToken);
    }
}
