namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages newsletter creation, scheduling, and sending operations.
    /// </summary>
    public class NewsletterManager : KeyedManager<Newsletter>, INewsletterManager
    {
        private readonly MobDbContext dbContext;
        private readonly IUserNewsletterPreferenceManager preferenceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterManager"/> class.
        /// </summary>
        /// <param name="repository">The newsletter repository.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="preferenceManager">The user preference manager.</param>
        public NewsletterManager(
            IKeyedRepository<Newsletter> repository,
            MobDbContext dbContext,
            IUserNewsletterPreferenceManager preferenceManager)
            : base(repository)
        {
            this.dbContext = dbContext;
            this.preferenceManager = preferenceManager;
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
    }
}
