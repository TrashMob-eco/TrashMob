namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
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
            var prospect = await prospectRepository.GetAsync(request.ProspectId, cancellationToken);
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

                // 5. Send welcome email if requested and contact email exists
                if (request.SendWelcomeEmail && !string.IsNullOrWhiteSpace(prospect.ContactEmail))
                {
                    await SendWelcomeEmailAsync(prospect, newPartner, cancellationToken);
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
            CommunityProspect prospect, Partner partner,
            CancellationToken cancellationToken)
        {
            try
            {
                var htmlCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.CommunityWelcome.ToString());
                htmlCopy = htmlCopy.Replace("{CommunityName}", partner.Name);

                var subject = $"Welcome to TrashMob.eco, {partner.Name}!";
                var dynamicTemplateData = new
                {
                    username = prospect.ContactName ?? prospect.Name,
                    emailCopy = htmlCopy,
                    subject,
                };

                List<Shared.Poco.EmailAddress> recipients =
                [
                    new()
                    {
                        Name = prospect.ContactName ?? prospect.Name,
                        Email = prospect.ContactEmail,
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
