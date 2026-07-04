namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="SalesReportNarrativeService"/> — the Project 63
    /// Phase 4 free-text sidecar for weekly and monthly reports.
    /// </summary>
    public class SalesReportNarrativeServiceTests : IDisposable
    {
        private readonly MobDbContext db;
        private readonly SalesReportNarrativeService sut;
        private readonly Guid userId = Guid.NewGuid();
        private readonly DateOnly weekStart = new DateOnly(2026, 7, 13);
        private readonly DateOnly weekEnd = new DateOnly(2026, 7, 19);
        private readonly DateOnly monthStart = new DateOnly(2026, 7, 1);
        private readonly DateOnly monthEnd = new DateOnly(2026, 7, 31);

        public SalesReportNarrativeServiceTests()
        {
            var options = new DbContextOptionsBuilder<MobDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalesReportNarrative_{Guid.NewGuid()}")
                .Options;
            db = new MobDbContext(options);
            sut = new SalesReportNarrativeService(db);
        }

        [Fact]
        public async Task Get_ReturnsNull_WhenNoRowExists()
        {
            var result = await sut.GetAsync(SalesReportPeriodTypeEnum.Weekly, weekStart);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpsertWeekly_CreatesRowWithNextStepsAndIgnoresPriority()
        {
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Weekly,
                weekStart,
                weekEnd,
                nextSteps: "Follow up on 3 warm cities",
                nextMonthPriority: "should be ignored on weekly",
                userId);

            var row = await sut.GetAsync(SalesReportPeriodTypeEnum.Weekly, weekStart);

            Assert.NotNull(row);
            Assert.Equal("Follow up on 3 warm cities", row!.NextSteps);
            Assert.Null(row.NextMonthPriority);
        }

        [Fact]
        public async Task UpsertMonthly_CreatesRowWithPriorityAndIgnoresNextSteps()
        {
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Monthly,
                monthStart,
                monthEnd,
                nextSteps: "should be ignored on monthly",
                nextMonthPriority: "Focus on Contra Costa County",
                userId);

            var row = await sut.GetAsync(SalesReportPeriodTypeEnum.Monthly, monthStart);

            Assert.NotNull(row);
            Assert.Equal("Focus on Contra Costa County", row!.NextMonthPriority);
            Assert.Null(row.NextSteps);
        }

        [Fact]
        public async Task Upsert_UpdatesExistingRowRatherThanDuplicating()
        {
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Weekly, weekStart, weekEnd,
                nextSteps: "v1", nextMonthPriority: null, userId);
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Weekly, weekStart, weekEnd,
                nextSteps: "v2 revised", nextMonthPriority: null, userId);

            var rows = await db.SalesReports
                .Where(r => r.PeriodType == (int)SalesReportPeriodTypeEnum.Weekly)
                .ToListAsync();

            Assert.Single(rows);
            Assert.Equal("v2 revised", rows[0].NextSteps);
        }

        [Fact]
        public async Task Upsert_NullNextSteps_ClearsExistingText()
        {
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Weekly, weekStart, weekEnd,
                nextSteps: "will be cleared", nextMonthPriority: null, userId);
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Weekly, weekStart, weekEnd,
                nextSteps: null, nextMonthPriority: null, userId);

            var row = await sut.GetAsync(SalesReportPeriodTypeEnum.Weekly, weekStart);

            Assert.NotNull(row);
            Assert.Null(row!.NextSteps);
        }

        [Fact]
        public async Task Upsert_WeeklyAndMonthlyForSameStartDate_AreDistinctRows()
        {
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Weekly, monthStart, monthStart.AddDays(6),
                nextSteps: "weekly text", nextMonthPriority: null, userId);
            await sut.UpsertAsync(
                SalesReportPeriodTypeEnum.Monthly, monthStart, monthEnd,
                nextSteps: null, nextMonthPriority: "monthly text", userId);

            var weekly = await sut.GetAsync(SalesReportPeriodTypeEnum.Weekly, monthStart);
            var monthly = await sut.GetAsync(SalesReportPeriodTypeEnum.Monthly, monthStart);

            Assert.NotNull(weekly);
            Assert.NotNull(monthly);
            Assert.NotEqual(weekly!.Id, monthly!.Id);
            Assert.Equal("weekly text", weekly.NextSteps);
            Assert.Equal("monthly text", monthly.NextMonthPriority);
        }

        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
