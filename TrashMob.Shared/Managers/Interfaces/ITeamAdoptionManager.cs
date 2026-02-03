namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing team adoption applications.
    /// </summary>
    public interface ITeamAdoptionManager : IKeyedManager<TeamAdoption>
    {
        /// <summary>
        /// Submits a new adoption application for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="adoptableAreaId">The adoptable area ID.</param>
        /// <param name="applicationNotes">Optional notes from the team about their application.</param>
        /// <param name="submittedByUserId">The user ID submitting the application.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the created adoption or an error.</returns>
        Task<ServiceResult<TeamAdoption>> SubmitApplicationAsync(
            Guid teamId,
            Guid adoptableAreaId,
            string applicationNotes,
            Guid submittedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves an adoption application.
        /// </summary>
        /// <param name="adoptionId">The adoption application ID.</param>
        /// <param name="reviewedByUserId">The user ID approving the application.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the approved adoption or an error.</returns>
        Task<ServiceResult<TeamAdoption>> ApproveApplicationAsync(
            Guid adoptionId,
            Guid reviewedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rejects an adoption application.
        /// </summary>
        /// <param name="adoptionId">The adoption application ID.</param>
        /// <param name="rejectionReason">The reason for rejection.</param>
        /// <param name="reviewedByUserId">The user ID rejecting the application.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the rejected adoption or an error.</returns>
        Task<ServiceResult<TeamAdoption>> RejectApplicationAsync(
            Guid adoptionId,
            string rejectionReason,
            Guid reviewedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all adoption applications for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of adoptions for the team.</returns>
        Task<IEnumerable<TeamAdoption>> GetByTeamIdAsync(
            Guid teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all adoption applications for an adoptable area.
        /// </summary>
        /// <param name="adoptableAreaId">The adoptable area ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of adoptions for the area.</returns>
        Task<IEnumerable<TeamAdoption>> GetByAreaIdAsync(
            Guid adoptableAreaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets pending adoption applications for a community (partner).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of pending adoptions for the community.</returns>
        Task<IEnumerable<TeamAdoption>> GetPendingByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets approved adoptions for a community (partner).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of approved adoptions for the community.</returns>
        Task<IEnumerable<TeamAdoption>> GetApprovedByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a team already has a pending or approved adoption for an area.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="adoptableAreaId">The adoptable area ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the team has an existing application; otherwise, false.</returns>
        Task<bool> HasExistingApplicationAsync(
            Guid teamId,
            Guid adoptableAreaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets delinquent adoptions for a community (teams not meeting cleanup requirements).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of non-compliant adoptions for the community.</returns>
        Task<IEnumerable<TeamAdoption>> GetDelinquentByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets compliance statistics for a community's adoption program.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>Compliance statistics for the community.</returns>
        Task<AdoptionComplianceStats> GetComplianceStatsByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all adoptions for a community with full details for export.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of all adoptions with details for the community.</returns>
        Task<IEnumerable<TeamAdoption>> GetAllForExportByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);
    }
}
