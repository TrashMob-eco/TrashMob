namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using EmailAddress = TrashMob.Shared.Poco.EmailAddress;

    /// <summary>
    /// Core business logic for prospect outreach emails including cadence management, send/preview, and follow-up processing.
    /// </summary>
    public class ProspectOutreachManager(
        IKeyedRepository<CommunityProspect> prospectRepository,
        IKeyedRepository<ProspectOutreachEmail> outreachEmailRepository,
        IKeyedRepository<ProspectActivity> activityRepository,
        IOutreachContentService contentService,
        IEmailManager emailManager,
        IConfiguration configuration,
        ILogger<ProspectOutreachManager> logger)
        : IProspectOutreachManager
    {
        private const int MaxBatchSize = 20;

        /// <inheritdoc />
        public async Task<OutreachPreview> PreviewOutreachAsync(Guid prospectId,
            CancellationToken cancellationToken = default)
        {
            var prospect = await prospectRepository.GetAsync(prospectId, cancellationToken);
            if (prospect is null)
            {
                return new OutreachPreview { ProspectId = prospectId, Subject = "Prospect not found" };
            }

            var nextStep = await GetNextCadenceStepAsync(prospectId, cancellationToken);
            if (nextStep > 4)
            {
                return new OutreachPreview
                {
                    ProspectId = prospectId,
                    ProspectName = prospect.Name,
                    CadenceStep = nextStep,
                    Subject = "Cadence complete — all 4 steps have been sent",
                };
            }

            return await contentService.GenerateOutreachContentAsync(prospect, nextStep, 0, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<OutreachSendResult> SendOutreachAsync(Guid prospectId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var settings = GetOutreachSettings();
            if (!settings.OutreachEnabled)
            {
                return new OutreachSendResult
                {
                    Success = false,
                    ErrorMessage = "Outreach is currently disabled. Enable it in configuration to send emails.",
                };
            }

            var prospect = await prospectRepository.GetAsync(prospectId, cancellationToken);
            if (prospect is null)
            {
                return new OutreachSendResult { Success = false, ErrorMessage = "Prospect not found." };
            }

            // Skip prospects in terminal pipeline stages (Active=5, Declined=6)
            if (prospect.PipelineStage >= 5)
            {
                return new OutreachSendResult
                {
                    Success = false,
                    ErrorMessage = $"Prospect is in stage {prospect.PipelineStage} and cannot receive outreach.",
                };
            }

            var recipientEmail = settings.TestMode ? settings.TestRecipientEmail : prospect.ContactEmail;
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return new OutreachSendResult
                {
                    Success = false,
                    ErrorMessage = "Prospect has no contact email address.",
                };
            }

            var nextStep = await GetNextCadenceStepAsync(prospectId, cancellationToken);
            if (nextStep > 4)
            {
                return new OutreachSendResult
                {
                    Success = false,
                    ErrorMessage = "All 4 cadence steps have been sent for this prospect.",
                };
            }

            try
            {
                // Generate AI content
                var preview = await contentService.GenerateOutreachContentAsync(
                    prospect, nextStep, 0, cancellationToken);

                // Get the email template shell and inject personalized content
                var notificationType = GetNotificationTypeForStep(nextStep);
                var templateHtml = emailManager.GetHtmlEmailCopy(notificationType.ToString());
                var fullHtml = templateHtml.Replace("{personalizedContent}", preview.HtmlBody);

                var subject = settings.TestMode ? $"[TEST] {preview.Subject}" : preview.Subject;

                // Create the outreach email record
                var outreachEmail = new ProspectOutreachEmail
                {
                    Id = Guid.NewGuid(),
                    ProspectId = prospectId,
                    CadenceStep = nextStep,
                    Subject = subject,
                    HtmlBody = fullHtml,
                    Status = ProspectOutreachStatus.Sent,
                    SentDate = DateTimeOffset.UtcNow,
                    CreatedByUserId = userId,
                    LastUpdatedByUserId = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastUpdatedDate = DateTimeOffset.UtcNow,
                };

                // Send via SendGrid
                var dynamicTemplateData = new
                {
                    subject,
                    emailCopy = fullHtml,
                };

                List<EmailAddress> recipients =
                [
                    new() { Name = prospect.ContactName ?? prospect.Name, Email = recipientEmail },
                ];

                await emailManager.SendTemplatedEmailAsync(
                    subject,
                    SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.ProspectOutreach,
                    dynamicTemplateData,
                    recipients,
                    cancellationToken);

                // Save the outreach email record
                await outreachEmailRepository.AddAsync(outreachEmail);

                // Log activity
                var activity = new ProspectActivity
                {
                    Id = Guid.NewGuid(),
                    ProspectId = prospectId,
                    ActivityType = "EmailSent",
                    Subject = $"Outreach Step {nextStep}: {subject}",
                    Details = settings.TestMode
                        ? $"Test email sent to {recipientEmail}"
                        : $"Email sent to {recipientEmail}",
                    CreatedByUserId = userId,
                    LastUpdatedByUserId = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastUpdatedDate = DateTimeOffset.UtcNow,
                };
                await activityRepository.AddAsync(activity);

                // Update prospect dates and pipeline stage
                prospect.LastContactedDate = DateTimeOffset.UtcNow;
                prospect.NextFollowUpDate = GetNextFollowUpDate(nextStep);
                prospect.LastUpdatedByUserId = userId;
                prospect.LastUpdatedDate = DateTimeOffset.UtcNow;

                // Advance from New (0) to Contacted (1) on first send
                if (prospect.PipelineStage == 0)
                {
                    prospect.PipelineStage = 1;
                }

                await prospectRepository.UpdateAsync(prospect);

                logger.LogInformation(
                    "Outreach email sent to prospect {ProspectId} step {Step} (test={TestMode})",
                    prospectId, nextStep, settings.TestMode);

                return new OutreachSendResult
                {
                    ProspectOutreachEmailId = outreachEmail.Id,
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send outreach email to prospect {ProspectId}", prospectId);
                return new OutreachSendResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to send: {ex.Message}",
                };
            }
        }

        /// <inheritdoc />
        public async Task<BatchOutreachResult> SendBatchOutreachAsync(List<Guid> prospectIds, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var result = new BatchOutreachResult
            {
                TotalRequested = prospectIds.Count,
            };

            var toProcess = prospectIds.Take(MaxBatchSize).ToList();
            result.Skipped = prospectIds.Count - toProcess.Count;

            foreach (var prospectId in toProcess)
            {
                var sendResult = await SendOutreachAsync(prospectId, userId, cancellationToken);
                result.Results.Add(sendResult);

                if (sendResult.Success)
                {
                    result.Sent++;
                }
                else if (sendResult.ErrorMessage?.Contains("disabled") == true ||
                         sendResult.ErrorMessage?.Contains("stage") == true ||
                         sendResult.ErrorMessage?.Contains("no contact") == true ||
                         sendResult.ErrorMessage?.Contains("cadence") == true)
                {
                    result.Skipped++;
                }
                else
                {
                    result.Failed++;
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProspectOutreachEmail>> GetOutreachHistoryAsync(Guid prospectId,
            CancellationToken cancellationToken = default)
        {
            return await outreachEmailRepository
                .Get(e => e.ProspectId == prospectId)
                .OrderByDescending(e => e.SentDate ?? e.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> ProcessDueFollowUpsAsync(CancellationToken cancellationToken = default)
        {
            var settings = GetOutreachSettings();
            if (!settings.OutreachEnabled)
            {
                logger.LogInformation("Outreach is disabled. Skipping follow-up processing.");
                return 0;
            }

            var now = DateTimeOffset.UtcNow;
            var dueProspects = await prospectRepository
                .Get(p => p.NextFollowUpDate != null
                    && p.NextFollowUpDate <= now
                    && p.PipelineStage == 1  // Contacted
                    && !string.IsNullOrWhiteSpace(p.ContactEmail))
                .Take(settings.MaxFollowUpsPerRun)
                .ToListAsync(cancellationToken);

            var processed = 0;

            foreach (var prospect in dueProspects)
            {
                // Use a system user ID for automated sends
                var result = await SendOutreachAsync(prospect.Id, Guid.Empty, cancellationToken);
                if (result.Success)
                {
                    processed++;
                }
                else
                {
                    logger.LogWarning(
                        "Follow-up failed for prospect {ProspectId}: {Error}",
                        prospect.Id, result.ErrorMessage);
                }
            }

            logger.LogInformation("Processed {Count} follow-up outreach emails", processed);
            return processed;
        }

        /// <inheritdoc />
        public OutreachSettings GetOutreachSettings()
        {
            return new OutreachSettings
            {
                OutreachEnabled = bool.TryParse(configuration["OutreachEnabled"], out var enabled) && enabled,
                TestMode = !bool.TryParse(configuration["OutreachTestMode"], out var testMode) || testMode,
                TestRecipientEmail = configuration["OutreachTestRecipientEmail"] ?? "info@trashmob.eco",
                MaxDailyOutreach = int.TryParse(configuration["OutreachMaxDailyOutreach"], out var maxDaily) ? maxDaily : 10,
                MaxFollowUpsPerRun = int.TryParse(configuration["OutreachMaxFollowUpsPerRun"], out var maxFollowUps) ? maxFollowUps : 10,
            };
        }

        private async Task<int> GetNextCadenceStepAsync(Guid prospectId, CancellationToken cancellationToken)
        {
            var lastStep = await outreachEmailRepository
                .Get(e => e.ProspectId == prospectId && e.Status != ProspectOutreachStatus.Failed)
                .MaxAsync(e => (int?)e.CadenceStep, cancellationToken) ?? 0;

            return lastStep + 1;
        }

        private static DateTimeOffset? GetNextFollowUpDate(int currentStep)
        {
            return currentStep switch
            {
                1 => DateTimeOffset.UtcNow.AddDays(3),   // Follow-up in 3 days
                2 => DateTimeOffset.UtcNow.AddDays(6),   // Value-add in 6 days
                3 => DateTimeOffset.UtcNow.AddDays(11),  // Final in 11 days
                4 => null,                                 // Cadence complete
                _ => null,
            };
        }

        private static NotificationTypeEnum GetNotificationTypeForStep(int cadenceStep)
        {
            return cadenceStep switch
            {
                1 => NotificationTypeEnum.ProspectOutreachInitial,
                2 => NotificationTypeEnum.ProspectOutreachFollowUp,
                3 => NotificationTypeEnum.ProspectOutreachValueAdd,
                4 => NotificationTypeEnum.ProspectOutreachFinal,
                _ => NotificationTypeEnum.ProspectOutreachInitial,
            };
        }
    }
}
