namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Generates and sends official participation report emails with PDF attachments.
    /// </summary>
    public class ParticipationReportService(
        IEventManager eventManager,
        IEventAttendeeMetricsManager metricsManager,
        IEventAttendeeManager attendeeManager,
        IUserManager userManager,
        IEmailManager emailManager,
        ILogger<ParticipationReportService> logger) : IParticipationReportService
    {
        /// <inheritdoc />
        public async Task<ServiceResult<bool>> SendReportAsync(Guid eventId, Guid userId,
            CancellationToken cancellationToken)
        {
            var evt = await eventManager.GetAsync(eventId, cancellationToken);
            if (evt == null)
            {
                return ServiceResult<bool>.Failure("Event not found.");
            }

            var metrics = await metricsManager.GetMyMetricsAsync(eventId, userId, cancellationToken);
            if (metrics == null)
            {
                return ServiceResult<bool>.Failure("No metrics submission found for this event.");
            }

            if (metrics.Status != EventAttendeeMetricsStatus.Approved &&
                metrics.Status != EventAttendeeMetricsStatus.Adjusted)
            {
                return ServiceResult<bool>.Failure("Your metrics have not been approved by the event lead yet.");
            }

            var user = await userManager.GetUserByInternalIdAsync(userId, cancellationToken);
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return ServiceResult<bool>.Failure("Could not find your account or email address.");
            }

            var reviewerName = await GetReviewerNameAsync(metrics, cancellationToken);

            await SendReportEmailAsync(evt, user, metrics, reviewerName, cancellationToken);

            logger.LogInformation("Participation report sent to user {UserId} for event {EventId}", userId, eventId);
            return ServiceResult<bool>.Success(true);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<int>> SendAllReportsAsync(Guid eventId, Guid requestingUserId,
            CancellationToken cancellationToken)
        {
            var isLead = await attendeeManager.IsEventLeadAsync(eventId, requestingUserId, cancellationToken);
            if (!isLead)
            {
                return ServiceResult<int>.Failure("Only event leads can send reports to all attendees.");
            }

            var evt = await eventManager.GetAsync(eventId, cancellationToken);
            if (evt == null)
            {
                return ServiceResult<int>.Failure("Event not found.");
            }

            var allMetrics = await metricsManager.GetByEventIdAsync(eventId, cancellationToken);
            var approvedMetrics = allMetrics
                .Where(m => m.Status is EventAttendeeMetricsStatus.Approved or EventAttendeeMetricsStatus.Adjusted)
                .ToList();

            if (approvedMetrics.Count == 0)
            {
                return ServiceResult<int>.Failure("No approved metrics found for this event.");
            }

            var sentCount = 0;
            foreach (var metrics in approvedMetrics)
            {
                try
                {
                    var user = await userManager.GetUserByInternalIdAsync(metrics.UserId, cancellationToken);
                    if (user == null || string.IsNullOrWhiteSpace(user.Email)) continue;

                    var reviewerName = await GetReviewerNameAsync(metrics, cancellationToken);
                    await SendReportEmailAsync(evt, user, metrics, reviewerName, cancellationToken);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to send participation report to user {UserId} for event {EventId}",
                        metrics.UserId, eventId);
                }
            }

            logger.LogInformation("Sent {Count} participation reports for event {EventId}", sentCount, eventId);
            return ServiceResult<int>.Success(sentCount);
        }

        private async Task SendReportEmailAsync(Event evt, User volunteer, EventAttendeeMetrics metrics,
            string reviewerName, CancellationToken cancellationToken)
        {
            // Generate PDF
            var pdfBytes = ParticipationReportPdfGenerator.Generate(evt, volunteer, metrics, reviewerName);

            // Build email body from template
            var emailCopy = emailManager.GetHtmlEmailCopy("ParticipationReport");
            var eventDate = evt.EventDate.ToLocalTime();

            var effectiveDuration = metrics.Status == "Adjusted" && metrics.AdjustedDurationMinutes.HasValue
                ? metrics.AdjustedDurationMinutes.Value
                : metrics.DurationMinutes ?? 0;
            var effectiveBags = metrics.Status == "Adjusted" && metrics.AdjustedBagsCollected.HasValue
                ? metrics.AdjustedBagsCollected.Value
                : metrics.BagsCollected ?? 0;
            var effectiveWeight = metrics.Status == "Adjusted" && metrics.AdjustedPickedWeight.HasValue
                ? metrics.AdjustedPickedWeight.Value
                : metrics.PickedWeight ?? 0;
            var effectiveWeightUnitId = metrics.Status == "Adjusted" && metrics.AdjustedPickedWeightUnitId.HasValue
                ? metrics.AdjustedPickedWeightUnitId.Value
                : metrics.PickedWeightUnitId ?? 1;

            var weightUnit = effectiveWeightUnitId == 2 ? "kg" : "lbs";
            var hours = effectiveDuration / 60;
            var minutes = effectiveDuration % 60;
            var durationText = hours > 0
                ? $"{hours} hour{(hours != 1 ? "s" : "")}{(minutes > 0 ? $" {minutes} min" : "")}"
                : $"{minutes} minutes";

            emailCopy = emailCopy.Replace("{eventName}", evt.Name);
            emailCopy = emailCopy.Replace("{eventDate}", eventDate.ToString("D"));
            emailCopy = emailCopy.Replace("{duration}", durationText);
            emailCopy = emailCopy.Replace("{bagsCollected}", effectiveBags.ToString());
            emailCopy = emailCopy.Replace("{weightPicked}", $"{effectiveWeight:F1} {weightUnit}");
            emailCopy = emailCopy.Replace("{verifiedBy}", reviewerName);

            var subject = $"Volunteer Participation Report — {evt.Name}";

            List<EmailAddress> recipients =
            [
                new() { Name = volunteer.DisplayFirstName, Email = volunteer.Email },
            ];

            var dynamicTemplateData = new
            {
                username = volunteer.DisplayFirstName,
                emailCopy,
                subject,
            };

            List<EmailAttachment> attachments =
            [
                new()
                {
                    Filename = $"TrashMob_Participation_Report_{evt.EventDate:yyyy-MM-dd}.pdf",
                    Base64Content = Convert.ToBase64String(pdfBytes),
                    MimeType = "application/pdf",
                },
            ];

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.EventRelated,
                dynamicTemplateData,
                recipients,
                attachments,
                cancellationToken);
        }

        private async Task<string> GetReviewerNameAsync(EventAttendeeMetrics metrics,
            CancellationToken cancellationToken)
        {
            if (metrics.ReviewedByUserId.HasValue)
            {
                var reviewer = await userManager.GetUserByInternalIdAsync(
                    metrics.ReviewedByUserId.Value, cancellationToken);
                if (reviewer != null)
                {
                    return reviewer.DisplayFirstName;
                }
            }

            return "Event Lead";
        }
    }
}
