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

    /// <summary>
    /// Manages user newsletter preferences including subscription status and initialization.
    /// </summary>
    public class UserNewsletterPreferenceManager : IUserNewsletterPreferenceManager
    {
        private readonly MobDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNewsletterPreferenceManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public UserNewsletterPreferenceManager(MobDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserNewsletterPreference>> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbContext.UserNewsletterPreferences
                .Include(p => p.Category)
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UserNewsletterPreference> UpdatePreferenceAsync(Guid userId, int categoryId, bool isSubscribed, CancellationToken cancellationToken = default)
        {
            var preference = await dbContext.UserNewsletterPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CategoryId == categoryId, cancellationToken);

            if (preference == null)
            {
                preference = new UserNewsletterPreference
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    IsSubscribed = isSubscribed,
                    SubscribedDate = isSubscribed ? DateTimeOffset.UtcNow : null,
                    UnsubscribedDate = isSubscribed ? null : DateTimeOffset.UtcNow
                };
                dbContext.UserNewsletterPreferences.Add(preference);
            }
            else
            {
                preference.IsSubscribed = isSubscribed;
                if (isSubscribed)
                {
                    preference.SubscribedDate = DateTimeOffset.UtcNow;
                    preference.UnsubscribedDate = null;
                }
                else
                {
                    preference.UnsubscribedDate = DateTimeOffset.UtcNow;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return preference;
        }

        /// <inheritdoc />
        public async Task UnsubscribeAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var preferences = await dbContext.UserNewsletterPreferences
                .Where(p => p.UserId == userId && p.IsSubscribed)
                .ToListAsync(cancellationToken);

            foreach (var preference in preferences)
            {
                preference.IsSubscribed = false;
                preference.UnsubscribedDate = DateTimeOffset.UtcNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task InitializePreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // Get default categories
            var defaultCategories = await dbContext.NewsletterCategories
                .Where(c => c.IsDefault && c.IsActive == true)
                .ToListAsync(cancellationToken);

            // Check which preferences already exist
            var existingCategoryIds = await dbContext.UserNewsletterPreferences
                .Where(p => p.UserId == userId)
                .Select(p => p.CategoryId)
                .ToListAsync(cancellationToken);

            // Create preferences for default categories that don't exist yet
            foreach (var category in defaultCategories.Where(c => !existingCategoryIds.Contains(c.Id)))
            {
                dbContext.UserNewsletterPreferences.Add(new UserNewsletterPreference
                {
                    UserId = userId,
                    CategoryId = category.Id,
                    IsSubscribed = true,
                    SubscribedDate = DateTimeOffset.UtcNow
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> GetSubscribedUsersAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await dbContext.UserNewsletterPreferences
                .Where(p => p.CategoryId == categoryId && p.IsSubscribed)
                .Select(p => p.UserId)
                .ToListAsync(cancellationToken);
        }
    }
}
