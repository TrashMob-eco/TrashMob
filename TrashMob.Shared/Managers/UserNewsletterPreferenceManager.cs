namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// Manages user newsletter preferences including subscription status and initialization.
    /// </summary>
    public class UserNewsletterPreferenceManager(MobDbContext dbContext, ILogger<UserNewsletterPreferenceManager> logger)
        : IUserNewsletterPreferenceManager
    {

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

        /// <inheritdoc />
        public string GenerateUnsubscribeToken(Guid userId, int? categoryId = null)
        {
            // Token format: userId|categoryId (categoryId is "all" if null)
            var categoryPart = categoryId.HasValue ? categoryId.Value.ToString() : "all";
            var payload = $"{userId}|{categoryPart}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload))
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        /// <inheritdoc />
        public async Task<UnsubscribeResult> ProcessUnsubscribeTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                // Decode the token
                var padded = token.Replace('-', '+').Replace('_', '/');
                switch (padded.Length % 4)
                {
                    case 2: padded += "=="; break;
                    case 3: padded += "="; break;
                }
                var payload = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
                var parts = payload.Split('|');

                if (parts.Length != 2 || !Guid.TryParse(parts[0], out var userId))
                {
                    return new UnsubscribeResult { Success = false, ErrorMessage = "Invalid token format." };
                }

                // Verify user exists
                var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
                if (user == null)
                {
                    return new UnsubscribeResult { Success = false, ErrorMessage = "User not found." };
                }

                var isAllCategories = parts[1] == "all";

                if (isAllCategories)
                {
                    await UnsubscribeAllAsync(userId, cancellationToken);
                    return new UnsubscribeResult
                    {
                        Success = true,
                        Email = MaskEmail(user.Email),
                        AllCategories = true
                    };
                }
                else
                {
                    if (!int.TryParse(parts[1], out var categoryId))
                    {
                        return new UnsubscribeResult { Success = false, ErrorMessage = "Invalid category in token." };
                    }

                    var category = await dbContext.NewsletterCategories.FindAsync(new object[] { categoryId }, cancellationToken);
                    if (category == null)
                    {
                        return new UnsubscribeResult { Success = false, ErrorMessage = "Category not found." };
                    }

                    await UpdatePreferenceAsync(userId, categoryId, false, cancellationToken);
                    return new UnsubscribeResult
                    {
                        Success = true,
                        Email = MaskEmail(user.Email),
                        AllCategories = false,
                        CategoryName = category.Name
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to process unsubscribe token");
                return new UnsubscribeResult { Success = false, ErrorMessage = "Invalid token." };
            }
        }

        /// <summary>
        /// Masks an email address for privacy (e.g., j***@example.com).
        /// </summary>
        private static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                return "***";
            }

            var parts = email.Split('@');
            var localPart = parts[0];
            var domainPart = parts[1];

            var maskedLocal = localPart.Length <= 2
                ? "***"
                : localPart[0] + "***" + localPart[^1];

            return $"{maskedLocal}@{domainPart}";
        }
    }
}
