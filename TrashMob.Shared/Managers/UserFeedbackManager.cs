namespace TrashMob.Shared.Managers
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
    /// Manages user feedback submissions, including email notifications to administrators.
    /// </summary>
    public class UserFeedbackManager : KeyedManager<UserFeedback>, IUserFeedbackManager
    {
        private readonly IEmailManager emailManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFeedbackManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for user feedback data access.</param>
        /// <param name="emailManager">The email manager for sending notifications.</param>
        public UserFeedbackManager(IKeyedRepository<UserFeedback> repository, IEmailManager emailManager)
            : base(repository)
        {
            this.emailManager = emailManager;
        }

        /// <inheritdoc />
        public override async Task<UserFeedback> AddAsync(UserFeedback feedback, CancellationToken cancellationToken = default)
        {
            // Handle anonymous submissions by using Guid.Empty for audit fields
            feedback.Id = Guid.NewGuid();
            feedback.LastUpdatedDate = DateTimeOffset.UtcNow;
            feedback.LastUpdatedByUserId = feedback.UserId ?? Guid.Empty;
            feedback.CreatedDate = DateTimeOffset.UtcNow;
            feedback.CreatedByUserId = feedback.UserId ?? Guid.Empty;
            feedback.Status = "New";

            var savedFeedback = await Repository.AddAsync(feedback);

            // Send email notification to admins
            await SendFeedbackNotificationAsync(savedFeedback, cancellationToken);

            return savedFeedback;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserFeedback>> GetByStatusAsync(string status = null, CancellationToken cancellationToken = default)
        {
            var query = Repository.Get();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(f => f.Status == status);
            }

            return await query
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserFeedback>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UserFeedback> UpdateStatusAsync(Guid id, string status, string internalNotes, Guid reviewedByUserId, CancellationToken cancellationToken = default)
        {
            var feedback = await Repo.GetAsync(id, cancellationToken);
            if (feedback == null)
            {
                return null;
            }

            feedback.Status = status;
            feedback.InternalNotes = internalNotes;
            feedback.ReviewedByUserId = reviewedByUserId;
            feedback.ReviewedDate = DateTimeOffset.UtcNow;
            feedback.LastUpdatedByUserId = reviewedByUserId;
            feedback.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await Repository.UpdateAsync(feedback);
        }

        private async Task SendFeedbackNotificationAsync(UserFeedback feedback, CancellationToken cancellationToken)
        {
            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.UserFeedbackReceived.ToString());
            var subject = $"New User Feedback Received: {feedback.Category}";

            var userName = feedback.UserId.HasValue ? "Registered User" : "Anonymous";
            var userEmail = feedback.Email ?? "Not provided";

            message = message.Replace("{Category}", feedback.Category);
            message = message.Replace("{UserName}", userName);
            message = message.Replace("{UserEmail}", userEmail);
            message = message.Replace("{PageUrl}", feedback.PageUrl ?? "Not provided");
            message = message.Replace("{Description}", feedback.Description);

            // Add screenshot section if available
            var screenshotSection = string.IsNullOrWhiteSpace(feedback.ScreenshotUrl)
                ? string.Empty
                : $"<p><strong>Screenshot:</strong> <a href=\"{feedback.ScreenshotUrl}\">View Screenshot</a></p>";
            message = message.Replace("{ScreenshotSection}", screenshotSection);

            var recipients = new List<EmailAddress>
            {
                new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
            };

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                emailCopy = message,
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
