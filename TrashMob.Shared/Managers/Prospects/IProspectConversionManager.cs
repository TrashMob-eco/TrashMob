namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Manages the conversion of community prospects to partner organizations.
    /// </summary>
    public interface IProspectConversionManager
    {
        /// <summary>
        /// Converts a community prospect to a partner organization.
        /// Creates the Partner, PartnerAdmin, updates the prospect, logs activity, and optionally sends welcome email.
        /// </summary>
        Task<ProspectConversionResult> ConvertToPartnerAsync(
            ProspectConversionRequest request, Guid userId,
            CancellationToken cancellationToken = default);
    }
}
