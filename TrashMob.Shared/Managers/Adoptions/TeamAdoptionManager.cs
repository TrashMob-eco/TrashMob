namespace TrashMob.Shared.Managers.Adoptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for team adoption application operations.
    /// </summary>
    public class TeamAdoptionManager : KeyedManager<TeamAdoption>, ITeamAdoptionManager
    {
        private readonly IEmailManager emailManager;
        private readonly IKeyedManager<Team> teamManager;
        private readonly ITeamMemberManager teamMemberManager;
        private readonly IAdoptableAreaManager adoptableAreaManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IPartnerAdminManager partnerAdminManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamAdoptionManager"/> class.
        /// </summary>
        public TeamAdoptionManager(
            IKeyedRepository<TeamAdoption> repository,
            IEmailManager emailManager,
            IKeyedManager<Team> teamManager,
            ITeamMemberManager teamMemberManager,
            IAdoptableAreaManager adoptableAreaManager,
            IKeyedManager<Partner> partnerManager,
            IPartnerAdminManager partnerAdminManager)
            : base(repository)
        {
            this.emailManager = emailManager;
            this.teamManager = teamManager;
            this.teamMemberManager = teamMemberManager;
            this.adoptableAreaManager = adoptableAreaManager;
            this.partnerManager = partnerManager;
            this.partnerAdminManager = partnerAdminManager;
        }

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoption>> SubmitApplicationAsync(
            Guid teamId,
            Guid adoptableAreaId,
            string applicationNotes,
            Guid submittedByUserId,
            CancellationToken cancellationToken = default)
        {
            // Validate team exists and is active
            var team = await teamManager.GetAsync(teamId, cancellationToken).ConfigureAwait(false);
            if (team == null || !team.IsActive)
            {
                return ServiceResult<TeamAdoption>.Failure("Team not found or is inactive.");
            }

            // Validate area exists, is active, and is available
            var area = await adoptableAreaManager.GetAsync(adoptableAreaId, cancellationToken).ConfigureAwait(false);
            if (area == null || !area.IsActive)
            {
                return ServiceResult<TeamAdoption>.Failure("Adoptable area not found or is inactive.");
            }

            if (area.Status != "Available" && !area.AllowCoAdoption)
            {
                return ServiceResult<TeamAdoption>.Failure("This area is not available for adoption.");
            }

            // Check for existing application
            if (await HasExistingApplicationAsync(teamId, adoptableAreaId, cancellationToken).ConfigureAwait(false))
            {
                return ServiceResult<TeamAdoption>.Failure("This team already has a pending or approved adoption for this area.");
            }

            // Create adoption application
            var adoption = new TeamAdoption
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                AdoptableAreaId = adoptableAreaId,
                ApplicationDate = DateTimeOffset.UtcNow,
                ApplicationNotes = applicationNotes,
                Status = "Pending",
                CreatedByUserId = submittedByUserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = submittedByUserId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            var result = await base.AddAsync(adoption, submittedByUserId, cancellationToken).ConfigureAwait(false);

            // Send notification to community admins
            await SendApplicationSubmittedNotificationAsync(result, team, area, cancellationToken).ConfigureAwait(false);

            return ServiceResult<TeamAdoption>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoption>> ApproveApplicationAsync(
            Guid adoptionId,
            Guid reviewedByUserId,
            CancellationToken cancellationToken = default)
        {
            var adoption = await Repo.GetAsync(adoptionId, cancellationToken).ConfigureAwait(false);
            if (adoption == null)
            {
                return ServiceResult<TeamAdoption>.Failure("Adoption application not found.");
            }

            if (adoption.Status != "Pending")
            {
                return ServiceResult<TeamAdoption>.Failure("Only pending applications can be approved.");
            }

            adoption.Status = "Approved";
            adoption.ReviewedByUserId = reviewedByUserId;
            adoption.ReviewedDate = DateTimeOffset.UtcNow;
            adoption.LastUpdatedByUserId = reviewedByUserId;
            adoption.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(adoption, reviewedByUserId, cancellationToken).ConfigureAwait(false);

            // Update area status if not allowing co-adoption
            var area = await adoptableAreaManager.GetAsync(adoption.AdoptableAreaId, cancellationToken).ConfigureAwait(false);
            if (area != null && !area.AllowCoAdoption)
            {
                area.Status = "Adopted";
                area.LastUpdatedByUserId = reviewedByUserId;
                area.LastUpdatedDate = DateTimeOffset.UtcNow;
                await adoptableAreaManager.UpdateAsync(area, reviewedByUserId, cancellationToken).ConfigureAwait(false);
            }

            // Send approval notification to team leads
            var team = await teamManager.GetAsync(adoption.TeamId, cancellationToken).ConfigureAwait(false);
            await SendApplicationApprovedNotificationAsync(result, team, area, cancellationToken).ConfigureAwait(false);

            return ServiceResult<TeamAdoption>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoption>> RejectApplicationAsync(
            Guid adoptionId,
            string rejectionReason,
            Guid reviewedByUserId,
            CancellationToken cancellationToken = default)
        {
            var adoption = await Repo.GetAsync(adoptionId, cancellationToken).ConfigureAwait(false);
            if (adoption == null)
            {
                return ServiceResult<TeamAdoption>.Failure("Adoption application not found.");
            }

            if (adoption.Status != "Pending")
            {
                return ServiceResult<TeamAdoption>.Failure("Only pending applications can be rejected.");
            }

            adoption.Status = "Rejected";
            adoption.ReviewedByUserId = reviewedByUserId;
            adoption.ReviewedDate = DateTimeOffset.UtcNow;
            adoption.RejectionReason = rejectionReason;
            adoption.LastUpdatedByUserId = reviewedByUserId;
            adoption.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(adoption, reviewedByUserId, cancellationToken).ConfigureAwait(false);

            // Send rejection notification to team leads
            var team = await teamManager.GetAsync(adoption.TeamId, cancellationToken).ConfigureAwait(false);
            var area = await adoptableAreaManager.GetAsync(adoption.AdoptableAreaId, cancellationToken).ConfigureAwait(false);
            await SendApplicationRejectedNotificationAsync(result, team, area, cancellationToken).ConfigureAwait(false);

            return ServiceResult<TeamAdoption>.Success(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetByTeamIdAsync(
            Guid teamId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.TeamId == teamId)
                .Include(a => a.AdoptableArea)
                    .ThenInclude(aa => aa.Partner)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetByAreaIdAsync(
            Guid adoptableAreaId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.AdoptableAreaId == adoptableAreaId)
                .Include(a => a.Team)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetPendingByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId && a.Status == "Pending")
                .Include(a => a.Team)
                .Include(a => a.AdoptableArea)
                .OrderBy(a => a.ApplicationDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetApprovedByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId && a.Status == "Approved")
                .Include(a => a.Team)
                .Include(a => a.AdoptableArea)
                .OrderByDescending(a => a.ReviewedDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> HasExistingApplicationAsync(
            Guid teamId,
            Guid adoptableAreaId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .AnyAsync(a => a.TeamId == teamId
                    && a.AdoptableAreaId == adoptableAreaId
                    && (a.Status == "Pending" || a.Status == "Approved"),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a notification to community admins when a team submits an adoption application.
        /// </summary>
        private async Task SendApplicationSubmittedNotificationAsync(
            TeamAdoption adoption,
            Team team,
            AdoptableArea area,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(area.PartnerId, cancellationToken).ConfigureAwait(false);
            var admins = await partnerAdminManager.GetAdminsForPartnerAsync(area.PartnerId, cancellationToken).ConfigureAwait(false);

            var adminList = admins.ToList();
            if (adminList.Count == 0)
            {
                return;
            }

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.AdoptionApplicationSubmitted.ToString());
            emailCopy = emailCopy.Replace("{TeamName}", team.Name);
            emailCopy = emailCopy.Replace("{AreaName}", area.Name);
            emailCopy = emailCopy.Replace("{AreaType}", area.AreaType);
            emailCopy = emailCopy.Replace("{ApplicationNotes}", string.IsNullOrWhiteSpace(adoption.ApplicationNotes) ? "No notes provided" : adoption.ApplicationNotes);

            var subject = $"New Adoption Application: {team.Name} for {area.Name}";

            var recipients = adminList
                .Where(a => !string.IsNullOrEmpty(a.Email))
                .Select(a => new EmailAddress { Name = a.UserName ?? a.Email, Email = a.Email })
                .ToList();

            if (recipients.Count > 0)
            {
                var dynamicTemplateData = new
                {
                    username = partner?.Name ?? "Community Admin",
                    emailCopy,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(
                    subject,
                    SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General,
                    dynamicTemplateData,
                    recipients,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends a notification to team leads when an adoption application is approved.
        /// </summary>
        private async Task SendApplicationApprovedNotificationAsync(
            TeamAdoption adoption,
            Team team,
            AdoptableArea area,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(area.PartnerId, cancellationToken).ConfigureAwait(false);
            var teamLeads = await teamMemberManager.GetTeamLeadsAsync(adoption.TeamId, cancellationToken).ConfigureAwait(false);

            var leadList = teamLeads.ToList();
            if (leadList.Count == 0)
            {
                return;
            }

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.AdoptionApplicationApproved.ToString());
            emailCopy = emailCopy.Replace("{TeamName}", team.Name);
            emailCopy = emailCopy.Replace("{AreaName}", area.Name);
            emailCopy = emailCopy.Replace("{CommunityName}", partner?.Name ?? "Community");
            emailCopy = emailCopy.Replace("{MinEventsPerYear}", area.MinEventsPerYear.ToString());
            emailCopy = emailCopy.Replace("{CleanupFrequencyDays}", area.CleanupFrequencyDays.ToString());
            emailCopy = emailCopy.Replace("{SafetyRequirements}", string.IsNullOrWhiteSpace(area.SafetyRequirements) ? "No specific safety requirements." : area.SafetyRequirements);

            var subject = $"Adoption Approved: {area.Name}";

            var recipients = leadList
                .Where(l => l.User != null && !string.IsNullOrEmpty(l.User.Email))
                .Select(l => new EmailAddress { Name = l.User.UserName ?? l.User.Email, Email = l.User.Email })
                .ToList();

            if (recipients.Count > 0)
            {
                var dynamicTemplateData = new
                {
                    username = team.Name,
                    emailCopy,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(
                    subject,
                    SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General,
                    dynamicTemplateData,
                    recipients,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends a notification to team leads when an adoption application is rejected.
        /// </summary>
        private async Task SendApplicationRejectedNotificationAsync(
            TeamAdoption adoption,
            Team team,
            AdoptableArea area,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(area.PartnerId, cancellationToken).ConfigureAwait(false);
            var teamLeads = await teamMemberManager.GetTeamLeadsAsync(adoption.TeamId, cancellationToken).ConfigureAwait(false);

            var leadList = teamLeads.ToList();
            if (leadList.Count == 0)
            {
                return;
            }

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.AdoptionApplicationRejected.ToString());
            emailCopy = emailCopy.Replace("{TeamName}", team.Name);
            emailCopy = emailCopy.Replace("{AreaName}", area.Name);
            emailCopy = emailCopy.Replace("{CommunityName}", partner?.Name ?? "Community");
            emailCopy = emailCopy.Replace("{RejectionReason}", string.IsNullOrWhiteSpace(adoption.RejectionReason) ? "No reason provided" : adoption.RejectionReason);

            var subject = $"Adoption Application Update: {area.Name}";

            var recipients = leadList
                .Where(l => l.User != null && !string.IsNullOrEmpty(l.User.Email))
                .Select(l => new EmailAddress { Name = l.User.UserName ?? l.User.Email, Email = l.User.Email })
                .ToList();

            if (recipients.Count > 0)
            {
                var dynamicTemplateData = new
                {
                    username = team.Name,
                    emailCopy,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(
                    subject,
                    SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General,
                    dynamicTemplateData,
                    recipients,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
