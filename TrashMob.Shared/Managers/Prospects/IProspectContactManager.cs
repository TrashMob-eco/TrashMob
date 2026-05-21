namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IProspectContactManager : IKeyedManager<ProspectContact>
    {
        /// <summary>
        /// Returns all contacts for a prospect, primary first.
        /// </summary>
        Task<IEnumerable<ProspectContact>> GetByProspectAsync(Guid prospectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atomically designates a contact as primary, clearing the IsPrimary flag on all
        /// other contacts for the same prospect.
        /// </summary>
        Task<ProspectContact> SetPrimaryAsync(Guid contactId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the lifecycle status of a contact (Active, WrongPerson, NoResponse,
        /// LeftOrganization, RightPerson).
        /// </summary>
        Task<ProspectContact> UpdateStatusAsync(Guid contactId, int newStatus, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns true if any ProspectActivity or ProspectOutreachEmail references this
        /// contact. Callers can use this to decide whether deletion should be allowed
        /// (hard delete) or blocked in favor of marking the contact inactive.
        /// </summary>
        Task<bool> HasReferencesAsync(Guid contactId, CancellationToken cancellationToken = default);
    }
}
