#nullable enable

namespace TrashMob.Shared.Managers.Prospects
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
    /// EF-backed subscriber list for the weekly and monthly sales pipeline
    /// emails (Project 63 Phase 4b).
    /// </summary>
    public class SalesReportSubscriberService(MobDbContext db) : ISalesReportSubscriberService
    {
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<SalesReportSubscriber>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await db.SalesReportSubscribers
                .Include(s => s.User)
                .OrderBy(s => s.User.UserName)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<SalesReportSubscriber>> GetForCadenceAsync(
            SalesReportPeriodTypeEnum periodType,
            CancellationToken cancellationToken = default)
        {
            var query = db.SalesReportSubscribers.Include(s => s.User).AsQueryable();
            query = periodType == SalesReportPeriodTypeEnum.Weekly
                ? query.Where(s => s.IncludeWeekly)
                : query.Where(s => s.IncludeMonthly);

            // Never fan out to accounts without an email address on file.
            return await query
                .Where(s => s.User.Email != null && s.User.Email != string.Empty)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<SalesReportSubscriber> AddOrUpdateAsync(
            Guid userId,
            bool includeWeekly,
            bool includeMonthly,
            Guid actingUserId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var existing = await db.SalesReportSubscribers
                .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

            if (existing != null)
            {
                if (existing.IncludeWeekly != includeWeekly || existing.IncludeMonthly != includeMonthly)
                {
                    existing.IncludeWeekly = includeWeekly;
                    existing.IncludeMonthly = includeMonthly;
                    existing.LastUpdatedByUserId = actingUserId;
                    existing.LastUpdatedDate = now;
                    await db.SaveChangesAsync(cancellationToken);
                }

                return existing;
            }

            var subscriber = new SalesReportSubscriber
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IncludeWeekly = includeWeekly,
                IncludeMonthly = includeMonthly,
                CreatedByUserId = actingUserId,
                CreatedDate = now,
                LastUpdatedByUserId = actingUserId,
                LastUpdatedDate = now,
            };
            db.SalesReportSubscribers.Add(subscriber);
            await db.SaveChangesAsync(cancellationToken);
            return subscriber;
        }

        /// <inheritdoc />
        public async Task<SalesReportSubscriber?> UpdateAsync(
            Guid subscriptionId,
            bool includeWeekly,
            bool includeMonthly,
            Guid actingUserId,
            CancellationToken cancellationToken = default)
        {
            var existing = await db.SalesReportSubscribers
                .FirstOrDefaultAsync(s => s.Id == subscriptionId, cancellationToken);
            if (existing == null)
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow;
            existing.IncludeWeekly = includeWeekly;
            existing.IncludeMonthly = includeMonthly;
            existing.LastUpdatedByUserId = actingUserId;
            existing.LastUpdatedDate = now;
            await db.SaveChangesAsync(cancellationToken);
            return existing;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var existing = await db.SalesReportSubscribers
                .FirstOrDefaultAsync(s => s.Id == subscriptionId, cancellationToken);
            if (existing == null)
            {
                return false;
            }

            db.SalesReportSubscribers.Remove(existing);
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
