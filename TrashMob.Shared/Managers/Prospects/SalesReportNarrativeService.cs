#nullable enable

namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// EF-backed narrative sidecar for weekly and monthly sales pipeline
    /// reports (Project 63 Phase 4).
    /// </summary>
    public class SalesReportNarrativeService(MobDbContext db) : ISalesReportNarrativeService
    {
        /// <inheritdoc />
        public async Task<SalesReport?> GetAsync(
            SalesReportPeriodTypeEnum periodType,
            DateOnly periodStart,
            CancellationToken cancellationToken = default)
        {
            var start = periodStart.ToDateTime(TimeOnly.MinValue);
            return await db.SalesReports
                .FirstOrDefaultAsync(
                    r => r.PeriodType == (int)periodType && r.PeriodStart == start,
                    cancellationToken);
        }

        /// <inheritdoc />
        public async Task<SalesReport> UpsertAsync(
            SalesReportPeriodTypeEnum periodType,
            DateOnly periodStart,
            DateOnly periodEnd,
            string? nextSteps,
            string? nextMonthPriority,
            Guid actingUserId,
            CancellationToken cancellationToken = default)
        {
            var start = periodStart.ToDateTime(TimeOnly.MinValue);
            var end = periodEnd.ToDateTime(TimeOnly.MinValue);
            var now = DateTimeOffset.UtcNow;

            var existing = await db.SalesReports
                .FirstOrDefaultAsync(
                    r => r.PeriodType == (int)periodType && r.PeriodStart == start,
                    cancellationToken);

            if (existing == null)
            {
                var row = new SalesReport
                {
                    Id = Guid.NewGuid(),
                    PeriodType = (int)periodType,
                    PeriodStart = start,
                    PeriodEnd = end,
                    NextSteps = periodType == SalesReportPeriodTypeEnum.Weekly ? nextSteps : null,
                    NextMonthPriority = periodType == SalesReportPeriodTypeEnum.Monthly ? nextMonthPriority : null,
                    CreatedByUserId = actingUserId,
                    CreatedDate = now,
                    LastUpdatedByUserId = actingUserId,
                    LastUpdatedDate = now,
                };
                db.SalesReports.Add(row);
                await db.SaveChangesAsync(cancellationToken);
                return row;
            }

            // Only the field relevant to this period type is written — a
            // caller mistakenly passing the wrong field name does not clobber
            // the good one.
            if (periodType == SalesReportPeriodTypeEnum.Weekly)
            {
                existing.NextSteps = nextSteps;
            }
            else
            {
                existing.NextMonthPriority = nextMonthPriority;
            }

            existing.PeriodEnd = end;
            existing.LastUpdatedByUserId = actingUserId;
            existing.LastUpdatedDate = now;
            await db.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
