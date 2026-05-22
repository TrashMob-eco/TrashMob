namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages the conversion of community prospects to partner organizations.
    /// </summary>
    public class ProspectConversionManager(
        IKeyedRepository<CommunityProspect> prospectRepository,
        IKeyedManager<Partner> partnerManager,
        IBaseManager<PartnerAdmin> partnerAdminManager,
        IKeyedRepository<ProspectActivity> activityRepository,
        IEmailManager emailManager,
        ILogger<ProspectConversionManager> logger)
        : IProspectConversionManager
    {

        public async Task<ProspectConversionResult> ConvertToPartnerAsync(
            ProspectConversionRequest request, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var prospect = await prospectRepository.Get()
                .Include(p => p.Contacts)
                .FirstOrDefaultAsync(p => p.Id == request.ProspectId, cancellationToken);
            if (prospect is null)
            {
                return new ProspectConversionResult { Success = false, ErrorMessage = "Prospect not found." };
            }

            if (prospect.ConvertedPartnerId.HasValue)
            {
                return new ProspectConversionResult { Success = false, ErrorMessage = "Prospect has already been converted to a partner." };
            }

            try
            {
                // 1. Create Partner from prospect data
                var partner = prospect.ToPartner(request.PartnerTypeId);
                var newPartner = await partnerManager.AddAsync(partner, userId, cancellationToken);

                // 2. Create PartnerAdmin linking current user
                var partnerAdmin = new PartnerAdmin
                {
                    PartnerId = newPartner.Id,
                    UserId = userId,
                };
                await partnerAdminManager.AddAsync(partnerAdmin, userId, cancellationToken);

                // 3. Update prospect: set ConvertedPartnerId, stage -> Active (5)
                prospect.ConvertedPartnerId = newPartner.Id;
                prospect.PipelineStage = 5;
                prospect.LastUpdatedByUserId = userId;
                prospect.LastUpdatedDate = DateTimeOffset.UtcNow;
                await prospectRepository.UpdateAsync(prospect);

                // 4. Log activity
                var activity = new ProspectActivity
                {
                    Id = Guid.NewGuid(),
                    ProspectId = prospect.Id,
                    ActivityType = "StatusChange",
                    Subject = "Converted to Partner",
                    Details = $"Created partner '{newPartner.Name}' (ID: {newPartner.Id})",
                    CreatedByUserId = userId,
                    LastUpdatedByUserId = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastUpdatedDate = DateTimeOffset.UtcNow,
                };
                await activityRepository.AddAsync(activity);

                // 5. Send welcome email if requested and a primary contact has an email
                var primaryContact = prospect.Contacts?.FirstOrDefault(c => c.IsPrimary)
                    ?? prospect.Contacts?.FirstOrDefault();
                if (request.SendWelcomeEmail && !string.IsNullOrWhiteSpace(primaryContact?.Email))
                {
                    await SendWelcomeEmailAsync(prospect, primaryContact, newPartner, cancellationToken);
                }

                logger.LogInformation("Converted prospect {ProspectId} to partner {PartnerId}", prospect.Id, newPartner.Id);

                return new ProspectConversionResult { Success = true, PartnerId = newPartner.Id };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to convert prospect {ProspectId} to partner", request.ProspectId);
                return new ProspectConversionResult
                {
                    Success = false,
                    ErrorMessage = $"Conversion failed: {ex.Message}",
                };
            }
        }

        private async Task SendWelcomeEmailAsync(
            CommunityProspect prospect, ProspectContact primaryContact, Partner partner,
            CancellationToken cancellationToken)
        {
            try
            {
                var htmlCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.CommunityWelcome.ToString());
                htmlCopy = htmlCopy.Replace("{CommunityName}", partner.Name);

                var subject = $"Welcome to TrashMob.eco, {partner.Name}!";
                var dynamicTemplateData = new
                {
                    username = primaryContact?.Name ?? prospect.Name,
                    emailCopy = htmlCopy,
                    subject,
                };

                List<Shared.Poco.EmailAddress> recipients =
                [
                    new()
                    {
                        Name = primaryContact?.Name ?? prospect.Name,
                        Email = primaryContact?.Email,
                    },
                ];

                await emailManager.SendTemplatedEmailAsync(
                    subject,
                    SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General,
                    dynamicTemplateData,
                    recipients,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send welcome email for partner {PartnerId}", partner.Id);
                // Don't fail the conversion if the welcome email fails
            }
        }
    }
}
