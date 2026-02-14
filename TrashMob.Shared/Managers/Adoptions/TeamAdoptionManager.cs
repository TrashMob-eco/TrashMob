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
    public class TeamAdoptionManager(
        IKeyedRepository<TeamAdoption> repository,
        IEmailManager emailManager,
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        IAdoptableAreaManager adoptableAreaManager,
        IKeyedManager<Partner> partnerManager,
        IPartnerAdminManager partnerAdminManager)
        : KeyedManager<TeamAdoption>(repository), ITeamAdoptionManager
    {

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoption>> SubmitApplicationAsync(
            Guid teamId,
            Guid adoptableAreaId,
            string applicationNotes,
            Guid submittedByUserId,
            CancellationToken cancellationToken = default)
        {
            // Validate team exists and is active
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return ServiceResult<TeamAdoption>.Failure("Team not found or is inactive.");
            }

            // Validate area exists, is active, and is available
            var area = await adoptableAreaManager.GetAsync(adoptableAreaId, cancellationToken);
            if (area is null || !area.IsActive)
            {
                return ServiceResult<TeamAdoption>.Failure("Adoptable area not found or is inactive.");
            }

            if (area.Status != "Available" && !area.AllowCoAdoption)
            {
                return ServiceResult<TeamAdoption>.Failure("This area is not available for adoption.");
            }

            // Check for existing application
            if (await HasExistingApplicationAsync(teamId, adoptableAreaId, cancellationToken))
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
                Status = TeamAdoptionStatus.Pending,
                CreatedByUserId = submittedByUserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = submittedByUserId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            var result = await base.AddAsync(adoption, submittedByUserId, cancellationToken);

            // Send notification to community admins
            await SendApplicationSubmittedNotificationAsync(result, team, area, cancellationToken);

            return ServiceResult<TeamAdoption>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoption>> ApproveApplicationAsync(
            Guid adoptionId,
            Guid reviewedByUserId,
            CancellationToken cancellationToken = default)
        {
            var adoption = await Repo.GetAsync(adoptionId, cancellationToken);
            if (adoption is null)
            {
                return ServiceResult<TeamAdoption>.Failure("Adoption application not found.");
            }

            if (adoption.Status != TeamAdoptionStatus.Pending)
            {
                return ServiceResult<TeamAdoption>.Failure("Only pending applications can be approved.");
            }

            adoption.Status = TeamAdoptionStatus.Approved;
            adoption.ReviewedByUserId = reviewedByUserId;
            adoption.ReviewedDate = DateTimeOffset.UtcNow;
            adoption.LastUpdatedByUserId = reviewedByUserId;
            adoption.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(adoption, reviewedByUserId, cancellationToken);

            // Update area status if not allowing co-adoption
            var area = await adoptableAreaManager.GetAsync(adoption.AdoptableAreaId, cancellationToken);
            if (area is not null && !area.AllowCoAdoption)
            {
                area.Status = "Adopted";
                area.LastUpdatedByUserId = reviewedByUserId;
                area.LastUpdatedDate = DateTimeOffset.UtcNow;
                await adoptableAreaManager.UpdateAsync(area, reviewedByUserId, cancellationToken);
            }

            // Send approval notification to team leads
            var team = await teamManager.GetAsync(adoption.TeamId, cancellationToken);
            await SendApplicationApprovedNotificationAsync(result, team, area, cancellationToken);

            return ServiceResult<TeamAdoption>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<TeamAdoption>> RejectApplicationAsync(
            Guid adoptionId,
            string rejectionReason,
            Guid reviewedByUserId,
            CancellationToken cancellationToken = default)
        {
            var adoption = await Repo.GetAsync(adoptionId, cancellationToken);
            if (adoption is null)
            {
                return ServiceResult<TeamAdoption>.Failure("Adoption application not found.");
            }

            if (adoption.Status != TeamAdoptionStatus.Pending)
            {
                return ServiceResult<TeamAdoption>.Failure("Only pending applications can be rejected.");
            }

            adoption.Status = TeamAdoptionStatus.Rejected;
            adoption.ReviewedByUserId = reviewedByUserId;
            adoption.ReviewedDate = DateTimeOffset.UtcNow;
            adoption.RejectionReason = rejectionReason;
            adoption.LastUpdatedByUserId = reviewedByUserId;
            adoption.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(adoption, reviewedByUserId, cancellationToken);

            // Send rejection notification to team leads
            var team = await teamManager.GetAsync(adoption.TeamId, cancellationToken);
            var area = await adoptableAreaManager.GetAsync(adoption.AdoptableAreaId, cancellationToken);
            await SendApplicationRejectedNotificationAsync(result, team, area, cancellationToken);

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
                .ToListAsync(cancellationToken);
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
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetPendingByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId && a.Status == TeamAdoptionStatus.Pending)
                .Include(a => a.Team)
                .Include(a => a.AdoptableArea)
                .OrderBy(a => a.ApplicationDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetApprovedByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId && a.Status == TeamAdoptionStatus.Approved)
                .Include(a => a.Team)
                .Include(a => a.AdoptableArea)
                .OrderByDescending(a => a.ReviewedDate)
                .ToListAsync(cancellationToken);
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
                    && (a.Status == TeamAdoptionStatus.Pending || a.Status == TeamAdoptionStatus.Approved),
                    cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetDelinquentByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId
                    && a.Status == TeamAdoptionStatus.Approved
                    && !a.IsCompliant
                    && (a.AdoptionEndDate == null || a.AdoptionEndDate >= today))
                .Include(a => a.Team)
                .Include(a => a.AdoptableArea)
                .OrderBy(a => a.LastEventDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AdoptionComplianceStats> GetComplianceStatsByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var adoptions = await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId
                    && a.Status == TeamAdoptionStatus.Approved
                    && (a.AdoptionEndDate == null || a.AdoptionEndDate >= today))
                .Include(a => a.AdoptableArea)
                .ToListAsync(cancellationToken);

            var areas = await adoptableAreaManager.GetByCommunityAsync(partnerId, cancellationToken);

            var activeAreas = areas.Where(a => a.IsActive).ToList();
            var adoptedAreaIds = adoptions.Select(a => a.AdoptableAreaId).Distinct().ToList();

            // Calculate at-risk (approaching delinquency - within 14 days of required cleanup)
            var atRiskCount = adoptions.Count(a => a.IsCompliant && IsApproachingDelinquency(a));

            return new AdoptionComplianceStats
            {
                TotalAdoptions = adoptions.Count,
                CompliantAdoptions = adoptions.Count(a => a.IsCompliant),
                AtRiskAdoptions = atRiskCount,
                DelinquentAdoptions = adoptions.Count(a => !a.IsCompliant),
                TotalAvailableAreas = activeAreas.Count,
                AdoptedAreas = activeAreas.Count(a => adoptedAreaIds.Contains(a.Id) || a.Status == "Adopted"),
                TotalLinkedEvents = adoptions.Sum(a => a.EventCount)
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamAdoption>> GetAllForExportByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.AdoptableArea.PartnerId == partnerId && a.Status == TeamAdoptionStatus.Approved)
                .Include(a => a.Team)
                .Include(a => a.AdoptableArea)
                .OrderBy(a => a.AdoptableArea.Name)
                .ThenBy(a => a.Team.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if an adoption is approaching delinquency (within 14 days of required cleanup).
        /// </summary>
        private static bool IsApproachingDelinquency(TeamAdoption adoption)
        {
            if (!adoption.LastEventDate.HasValue)
            {
                // Check if approaching first event deadline
                if (!adoption.AdoptionStartDate.HasValue)
                {
                    return false;
                }

                var daysSinceStart = (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - adoption.AdoptionStartDate.Value.DayNumber);
                var gracePeriod = 90; // 90 days for first event
                return daysSinceStart >= (gracePeriod - 14) && daysSinceStart < gracePeriod;
            }

            var daysSinceLastEvent = (DateTimeOffset.UtcNow - adoption.LastEventDate.Value).TotalDays;
            var requiredFrequency = adoption.AdoptableArea?.CleanupFrequencyDays ?? 90;

            // At-risk if within 14 days of deadline (but not past it yet)
            return daysSinceLastEvent >= (requiredFrequency - 14) && daysSinceLastEvent < requiredFrequency;
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
            var partner = await partnerManager.GetAsync(area.PartnerId, cancellationToken);
            var admins = await partnerAdminManager.GetAdminsForPartnerAsync(area.PartnerId, cancellationToken);

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
                .Where(a => !string.IsNullOrWhiteSpace(a.Email))
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
                    cancellationToken);
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
            var partner = await partnerManager.GetAsync(area.PartnerId, cancellationToken);
            var teamLeads = await teamMemberManager.GetTeamLeadsAsync(adoption.TeamId, cancellationToken);

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
                .Where(l => l.User is not null && !string.IsNullOrWhiteSpace(l.User.Email))
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
                    cancellationToken);
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
            var partner = await partnerManager.GetAsync(area.PartnerId, cancellationToken);
            var teamLeads = await teamMemberManager.GetTeamLeadsAsync(adoption.TeamId, cancellationToken);

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
                .Where(l => l.User is not null && !string.IsNullOrWhiteSpace(l.User.Email))
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
                    cancellationToken);
            }
        }
    }
}
