namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Manages user assignments to professional cleanup companies.
    /// </summary>
    public interface IProfessionalCompanyUserManager : IBaseManager<ProfessionalCompanyUser>
    {
        /// <summary>
        /// Gets all users assigned to a professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of users.</returns>
        Task<IEnumerable<User>> GetUsersForCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all professional companies a user is assigned to.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of professional companies.</returns>
        Task<IEnumerable<ProfessionalCompany>> GetCompaniesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a user is assigned to a professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the user is assigned to the company.</returns>
        Task<bool> IsCompanyUserAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);
    }
}
