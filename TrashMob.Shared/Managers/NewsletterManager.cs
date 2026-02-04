namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages newsletter creation, scheduling, and sending operations.
    /// </summary>
    public class NewsletterManager : KeyedManager<Newsletter>, INewsletterManager
    {
        private readonly MobDbContext dbContext;
        private readonly IUserNewsletterPreferenceManager preferenceManager;
        private readonly IEmailManager emailManager;
        private readonly ILogger<NewsletterManager> logger;

        /// <summary>
        /// Batch size for sending emails.
        /// </summary>
        private const int BatchSize = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterManager"/> class.
        /// </summary>
        /// <param name="repository">The newsletter repository.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="preferenceManager">The user preference manager.</param>
        /// <param name="emailManager">The email manager.</param>
        /// <param name="logger">The logger.</param>
        public NewsletterManager(
            IKeyedRepository<Newsletter> repository,
            MobDbContext dbContext,
            IUserNewsletterPreferenceManager preferenceManager,
            IEmailManager emailManager,
            ILogger<NewsletterManager> logger)
            : base(repository)
        {
            this.dbContext = dbContext;
            this.preferenceManager = preferenceManager;
            this.emailManager = emailManager;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Newsletter>> GetNewslettersAsync(string status = null, CancellationToken cancellationToken = default)
        {
            var query = dbContext.Newsletters
                .Include(n => n.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(n => n.Status == status);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Newsletter>> GetScheduledNewslettersAsync(DateTimeOffset beforeDate, CancellationToken cancellationToken = default)
        {
            return await dbContext.Newsletters
                .Where(n => n.Status == "Scheduled" && n.ScheduledDate <= beforeDate)
                .Include(n => n.Category)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Newsletter> ScheduleNewsletterAsync(Guid newsletterId, DateTimeOffset scheduledDate, Guid userId, CancellationToken cancellationToken = default)
        {
            var newsletter = await GetAsync(newsletterId, cancellationToken);
            if (newsletter == null)
            {
                throw new InvalidOperationException($"Newsletter {newsletterId} not found.");
            }

            if (newsletter.Status != "Draft")
            {
                throw new InvalidOperationException($"Only draft newsletters can be scheduled. Current status: {newsletter.Status}");
            }

            newsletter.Status = "Scheduled";
            newsletter.ScheduledDate = scheduledDate;
            newsletter.LastUpdatedByUserId = userId;
            newsletter.LastUpdatedDate = DateTimeOffset.UtcNow;

            await UpdateAsync(newsletter, userId, cancellationToken);
            return newsletter;
        }

        /// <inheritdoc />
        public async Task<Newsletter> SendNewsletterAsync(Guid newsletterId, Guid userId, CancellationToken cancellationToken = default)
        {
            var newsletter = await dbContext.Newsletters
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == newsletterId, cancellationToken);

            if (newsletter == null)
            {
                throw new InvalidOperationException($"Newsletter {newsletterId} not found.");
            }

            if (newsletter.Status != "Draft" && newsletter.Status != "Scheduled")
            {
                throw new InvalidOperationException($"Newsletter cannot be sent. Current status: {newsletter.Status}");
            }

            // Get recipients and update count
            var recipients = await GetRecipientsAsync(newsletter, cancellationToken);
            newsletter.RecipientCount = recipients.Count();

            newsletter.Status = "Sending";
            newsletter.LastUpdatedByUserId = userId;
            newsletter.LastUpdatedDate = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return newsletter;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetRecipientsAsync(Newsletter newsletter, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query;

            switch (newsletter.TargetType)
            {
                case "Team":
                    if (!newsletter.TargetId.HasValue)
                    {
                        throw new InvalidOperationException("Team newsletter requires a TargetId.");
                    }
                    // Get team members
                    query = dbContext.TeamMembers
                        .Where(tm => tm.TeamId == newsletter.TargetId.Value)
                        .Select(tm => tm.User);
                    break;

                case "Community":
                    if (!newsletter.TargetId.HasValue)
                    {
                        throw new InvalidOperationException("Community newsletter requires a TargetId.");
                    }
                    // Get users in the community's location
                    var community = await dbContext.Partners
                        .FirstOrDefaultAsync(p => p.Id == newsletter.TargetId.Value, cancellationToken);

                    if (community == null)
                    {
                        return Enumerable.Empty<User>();
                    }

                    query = dbContext.Users
                        .Where(u => u.City == community.City && u.Region == community.Region);
                    break;

                case "All":
                default:
                    query = dbContext.Users.AsQueryable();
                    break;
            }

            // Filter by category subscription
            var subscribedUserIds = await preferenceManager.GetSubscribedUsersAsync(newsletter.CategoryId, cancellationToken);
            var subscribedSet = subscribedUserIds.ToHashSet();

            var users = await query.ToListAsync(cancellationToken);
            return users.Where(u => subscribedSet.Contains(u.Id) && !string.IsNullOrEmpty(u.Email));
        }

        /// <inheritdoc />
        public async Task UpdateStatisticsAsync(Guid newsletterId, int deliveredCount, int openCount, int clickCount, int bounceCount, int unsubscribeCount, CancellationToken cancellationToken = default)
        {
            var newsletter = await dbContext.Newsletters.FindAsync(new object[] { newsletterId }, cancellationToken);
            if (newsletter == null)
            {
                return;
            }

            newsletter.DeliveredCount = deliveredCount;
            newsletter.OpenCount = openCount;
            newsletter.ClickCount = clickCount;
            newsletter.BounceCount = bounceCount;
            newsletter.UnsubscribeCount = unsubscribeCount;

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendTestEmailAsync(Guid newsletterId, IEnumerable<string> testEmails, CancellationToken cancellationToken = default)
        {
            var newsletter = await dbContext.Newsletters
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == newsletterId, cancellationToken);

            if (newsletter == null)
            {
                throw new InvalidOperationException($"Newsletter {newsletterId} not found.");
            }

            var recipients = testEmails
                .Select(email => new EmailAddress { Email = email, Name = "Test Recipient" })
                .ToList();

            var templateData = BuildTemplateData(newsletter);

            await emailManager.SendTemplatedEmailAsync(
                $"[TEST] {newsletter.Subject}",
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                templateData,
                recipients,
                cancellationToken);

            logger.LogInformation("Test email sent for newsletter {NewsletterId} to {Count} recipients", newsletterId, recipients.Count);
        }

        /// <inheritdoc />
        public async Task<int> ProcessScheduledNewslettersAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var scheduledNewsletters = await GetScheduledNewslettersAsync(now, cancellationToken);
            var count = 0;

            foreach (var newsletter in scheduledNewsletters)
            {
                try
                {
                    // Get recipients and update count
                    var recipients = await GetRecipientsAsync(newsletter, cancellationToken);
                    newsletter.RecipientCount = recipients.Count();
                    newsletter.Status = "Sending";
                    newsletter.LastUpdatedDate = DateTimeOffset.UtcNow;

                    await dbContext.SaveChangesAsync(cancellationToken);
                    count++;

                    logger.LogInformation("Newsletter {NewsletterId} started sending to {RecipientCount} recipients",
                        newsletter.Id, newsletter.RecipientCount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to start sending newsletter {NewsletterId}", newsletter.Id);
                }
            }

            return count;
        }

        /// <inheritdoc />
        public async Task<int> ProcessSendingNewslettersAsync(CancellationToken cancellationToken = default)
        {
            var sendingNewsletters = await dbContext.Newsletters
                .Where(n => n.Status == "Sending")
                .Include(n => n.Category)
                .ToListAsync(cancellationToken);

            var processedCount = 0;

            foreach (var newsletter in sendingNewsletters)
            {
                try
                {
                    await SendNewsletterEmailsAsync(newsletter, cancellationToken);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send newsletter {NewsletterId}", newsletter.Id);
                    newsletter.Status = "Failed";
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            return processedCount;
        }

        /// <summary>
        /// Sends all emails for a newsletter in batches.
        /// </summary>
        private async Task SendNewsletterEmailsAsync(Newsletter newsletter, CancellationToken cancellationToken)
        {
            var recipients = (await GetRecipientsAsync(newsletter, cancellationToken)).ToList();
            var templateData = BuildTemplateData(newsletter);
            var sentCount = 0;

            // Process in batches
            for (var i = 0; i < recipients.Count; i += BatchSize)
            {
                var batch = recipients
                    .Skip(i)
                    .Take(BatchSize)
                    .Select(u => new EmailAddress
                    {
                        Email = u.Email,
                        Name = u.UserName ?? u.Email
                    })
                    .ToList();

                try
                {
                    await emailManager.SendTemplatedEmailAsync(
                        newsletter.Subject,
                        SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General,
                        templateData,
                        batch,
                        cancellationToken);

                    sentCount += batch.Count;

                    // Update progress periodically
                    if (i % (BatchSize * 5) == 0 || i + BatchSize >= recipients.Count)
                    {
                        newsletter.SentCount = sentCount;
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }

                    logger.LogInformation("Sent batch {BatchNumber} ({BatchCount} emails) for newsletter {NewsletterId}",
                        i / BatchSize + 1, batch.Count, newsletter.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send batch {BatchNumber} for newsletter {NewsletterId}", i / BatchSize + 1, newsletter.Id);
                }
            }

            // Mark as sent
            newsletter.SentCount = sentCount;
            newsletter.Status = "Sent";
            newsletter.SentDate = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Newsletter {NewsletterId} completed sending. {SentCount} of {RecipientCount} emails sent.",
                newsletter.Id, sentCount, newsletter.RecipientCount);
        }

        /// <summary>
        /// Builds the dynamic template data for the newsletter email.
        /// </summary>
        private static object BuildTemplateData(Newsletter newsletter)
        {
            return new
            {
                subject = newsletter.Subject,
                previewText = newsletter.PreviewText ?? string.Empty,
                htmlBody = newsletter.HtmlContent ?? string.Empty,
                textBody = newsletter.TextContent ?? string.Empty,
                categoryName = newsletter.Category?.Name ?? "Newsletter"
            };
        }
    }
}
