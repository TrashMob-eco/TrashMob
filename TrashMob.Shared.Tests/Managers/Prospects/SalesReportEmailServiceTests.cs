namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Poco;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="SalesReportEmailService"/> — Project 63 Phase 4b.
    /// Focuses on the day-of-week / day-of-month gate, empty-window skip,
    /// subscriber-count skip, and the EmailSentDate dedupe path so re-running
    /// the daily job on the same day is a no-op.
    /// </summary>
    public class SalesReportEmailServiceTests : IDisposable
    {
        private readonly MobDbContext db;
        private readonly Mock<IWeeklySalesReportService> weekly = new();
        private readonly Mock<IMonthlySalesReportService> monthly = new();
        private readonly Mock<ISalesReportSubscriberService> subscribers = new();
        private readonly Mock<IEmailManager> emailManager = new();
        private readonly Mock<ILogger<SalesReportEmailService>> logger = new();

        // Monday 2026-07-13 00:00 UTC — the first Monday after the salesperson's
        // start date, and the anchor date used across the weekly-path tests.
        private readonly DateTimeOffset monday = new(2026, 7, 13, 0, 0, 0, TimeSpan.Zero);

        // First-of-month anchor for the monthly-path tests.
        private readonly DateTimeOffset firstOfMonth = new(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);

        // A non-send day: Tuesday, mid-month.
        private readonly DateTimeOffset tuesday = new(2026, 7, 14, 0, 0, 0, TimeSpan.Zero);

        public SalesReportEmailServiceTests()
        {
            var options = new DbContextOptionsBuilder<MobDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalesReportEmail_{Guid.NewGuid()}")
                .Options;
            db = new MobDbContext(options);
        }

        private SalesReportEmailService BuildSut(DateTimeOffset now)
        {
            var clock = new FakeTimeProvider(now);
            return new SalesReportEmailService(
                db,
                weekly.Object,
                monthly.Object,
                subscribers.Object,
                emailManager.Object,
                logger.Object,
                clock);
        }

        private static WeeklySalesReportDto NonEmptyWeekly() => new()
        {
            OutreachTouches = 3,
        };

        private static WeeklySalesReportDto EmptyWeekly() => new();

        private static MonthlySalesReportDto NonEmptyMonthly() => new()
        {
            Metrics =
            [
                new MonthlySalesMetricDto { Metric = 3, Label = "Outreach touches", Target = 15, Actual = 8, Status = "OnTrack" },
            ],
        };

        private static MonthlySalesReportDto EmptyMonthly() => new()
        {
            Metrics =
            [
                new MonthlySalesMetricDto { Metric = 3, Label = "Outreach touches", Target = 15, Actual = 0, Status = "Behind" },
            ],
        };

        private static List<SalesReportSubscriber> OneSubscriber() =>
        [
            new SalesReportSubscriber
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                IncludeWeekly = true,
                IncludeMonthly = true,
                User = new User { Id = Guid.NewGuid(), UserName = "cynthia", Email = "cynthia@test.com" },
            },
        ];

        private void SetupTemplateFetch()
        {
            emailManager
                .Setup(m => m.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{PeriodLabel}</p>");
        }

        // ---------- Weekly ----------

        [Fact]
        public async Task Weekly_NotMonday_ReturnsZero_NoSideEffects()
        {
            var sut = BuildSut(tuesday);

            var sent = await sut.SendWeeklyReportIfDueAsync();

            Assert.Equal(0, sent);
            weekly.Verify(w => w.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
            emailManager.Verify(m => m.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Empty(db.SalesReports);
        }

        [Fact]
        public async Task Weekly_MondayEmptyReport_ReturnsZero_NoDedupeRow()
        {
            weekly.Setup(w => w.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmptyWeekly());

            var sut = BuildSut(monday);
            var sent = await sut.SendWeeklyReportIfDueAsync();

            Assert.Equal(0, sent);
            emailManager.Verify(m => m.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Empty(db.SalesReports);
        }

        [Fact]
        public async Task Weekly_MondayNoSubscribers_ReturnsZero_NoDedupeRow()
        {
            weekly.Setup(w => w.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(NonEmptyWeekly());
            subscribers.Setup(s => s.GetForCadenceAsync(
                    SalesReportPeriodTypeEnum.Weekly, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var sut = BuildSut(monday);
            var sent = await sut.SendWeeklyReportIfDueAsync();

            Assert.Equal(0, sent);
            Assert.Empty(db.SalesReports);
        }

        [Fact]
        public async Task Weekly_HappyPath_SendsToEachSubscriberAndMarksSent()
        {
            weekly.Setup(w => w.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(NonEmptyWeekly());
            subscribers.Setup(s => s.GetForCadenceAsync(
                    SalesReportPeriodTypeEnum.Weekly, It.IsAny<CancellationToken>()))
                .ReturnsAsync(OneSubscriber());
            SetupTemplateFetch();

            var sut = BuildSut(monday);
            var sent = await sut.SendWeeklyReportIfDueAsync();

            Assert.Equal(1, sent);
            emailManager.Verify(m => m.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.Is<List<EmailAddress>>(r => r.Count == 1 && r[0].Email == "cynthia@test.com"),
                It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.Single(db.SalesReports);
            Assert.NotNull(db.SalesReports.First().EmailSentDate);
        }

        [Fact]
        public async Task Weekly_SecondFiringSameDay_IsIdempotent()
        {
            // Seed a "sent" dedupe row for the just-ended week.
            var weekEnding = DateOnly.FromDateTime(monday.UtcDateTime).AddDays(-1);
            var weekStart = weekEnding.AddDays(-6);
            db.SalesReports.Add(new SalesReport
            {
                Id = Guid.NewGuid(),
                PeriodType = (int)SalesReportPeriodTypeEnum.Weekly,
                PeriodStart = weekStart.ToDateTime(TimeOnly.MinValue),
                PeriodEnd = weekEnding.ToDateTime(TimeOnly.MinValue),
                EmailSentDate = monday,
                CreatedDate = monday,
                LastUpdatedDate = monday,
            });
            db.SaveChanges();

            var sut = BuildSut(monday);
            var sent = await sut.SendWeeklyReportIfDueAsync();

            Assert.Equal(0, sent);
            weekly.Verify(w => w.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
            emailManager.Verify(m => m.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ---------- Monthly ----------

        [Fact]
        public async Task Monthly_NotFirstOfMonth_ReturnsZero_NoSideEffects()
        {
            var sut = BuildSut(monday); // 13th, not the 1st

            var sent = await sut.SendMonthlyReportIfDueAsync();

            Assert.Equal(0, sent);
            monthly.Verify(m => m.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Monthly_FirstOfMonthEmpty_ReturnsZero()
        {
            monthly.Setup(m => m.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmptyMonthly());

            var sut = BuildSut(firstOfMonth);
            var sent = await sut.SendMonthlyReportIfDueAsync();

            Assert.Equal(0, sent);
            Assert.Empty(db.SalesReports);
        }

        [Fact]
        public async Task Monthly_HappyPath_SendsAndMarksSent()
        {
            monthly.Setup(m => m.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(NonEmptyMonthly());
            subscribers.Setup(s => s.GetForCadenceAsync(
                    SalesReportPeriodTypeEnum.Monthly, It.IsAny<CancellationToken>()))
                .ReturnsAsync(OneSubscriber());
            SetupTemplateFetch();

            var sut = BuildSut(firstOfMonth);
            var sent = await sut.SendMonthlyReportIfDueAsync();

            Assert.Equal(1, sent);
            var row = Assert.Single(db.SalesReports);
            Assert.Equal((int)SalesReportPeriodTypeEnum.Monthly, row.PeriodType);
            // Previous month → July 2026
            Assert.Equal(new DateTime(2026, 7, 1), row.PeriodStart);
            Assert.NotNull(row.EmailSentDate);
        }

        [Fact]
        public async Task Monthly_SecondFiringSameDay_IsIdempotent()
        {
            var previousMonthStart = new DateOnly(2026, 7, 1);
            db.SalesReports.Add(new SalesReport
            {
                Id = Guid.NewGuid(),
                PeriodType = (int)SalesReportPeriodTypeEnum.Monthly,
                PeriodStart = previousMonthStart.ToDateTime(TimeOnly.MinValue),
                PeriodEnd = previousMonthStart.AddMonths(1).AddDays(-1).ToDateTime(TimeOnly.MinValue),
                EmailSentDate = firstOfMonth,
                CreatedDate = firstOfMonth,
                LastUpdatedDate = firstOfMonth,
            });
            db.SaveChanges();

            var sut = BuildSut(firstOfMonth);
            var sent = await sut.SendMonthlyReportIfDueAsync();

            Assert.Equal(0, sent);
            monthly.Verify(m => m.GenerateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Trivial <see cref="TimeProvider"/> stub — the service's clock is the
        /// only piece of ambient state under test, so a two-line override is
        /// simpler than pulling in Microsoft.Extensions.Time.Testing.
        /// </summary>
        private sealed class FakeTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }
    }
}
