namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="WeeklySalesReportService"/> — the Project 63 Phase 2
    /// aggregation. Covers window boundaries, activity-type case-insensitivity,
    /// touched-prospect feedback fan-out, and the empty-window path.
    /// </summary>
    public class WeeklySalesReportServiceTests : IDisposable
    {
        private readonly MobDbContext db;
        private readonly WeeklySalesReportService sut;
        private readonly DateOnly weekEnding = new DateOnly(2026, 7, 12); // Sunday
        private readonly DateTimeOffset windowStart = new DateTimeOffset(2026, 7, 6, 0, 0, 0, TimeSpan.Zero);
        private readonly DateTimeOffset windowMid = new DateTimeOffset(2026, 7, 9, 12, 0, 0, TimeSpan.Zero);
        private readonly DateTimeOffset windowEnd = new DateTimeOffset(2026, 7, 12, 23, 0, 0, TimeSpan.Zero);
        private readonly DateTimeOffset outsideWindow = new DateTimeOffset(2026, 7, 5, 12, 0, 0, TimeSpan.Zero);

        public WeeklySalesReportServiceTests()
        {
            var options = new DbContextOptionsBuilder<MobDbContext>()
                .UseInMemoryDatabase(databaseName: $"WeeklySalesReport_{Guid.NewGuid()}")
                .Options;
            db = new MobDbContext(options);
            sut = new WeeklySalesReportService(db);
        }

        private CommunityProspect AddProspect(
            DateTimeOffset createdDate,
            string keyObjection = null,
            string pricingFeedback = null)
        {
            var prospect = new CommunityProspect
            {
                Id = Guid.NewGuid(),
                Name = "Test City",
                CreatedDate = createdDate,
                LastUpdatedDate = createdDate,
                KeyObjection = keyObjection,
                PricingFeedback = pricingFeedback,
            };
            db.CommunityProspects.Add(prospect);
            db.SaveChanges();
            return prospect;
        }

        private void AddActivity(Guid prospectId, string activityType, DateTimeOffset createdDate)
        {
            db.ProspectActivities.Add(new ProspectActivity
            {
                Id = Guid.NewGuid(),
                ProspectId = prospectId,
                ActivityType = activityType,
                Subject = "test",
                CreatedDate = createdDate,
                LastUpdatedDate = createdDate,
            });
            db.SaveChanges();
        }

        private void AddContact(Guid prospectId, DateTimeOffset createdDate)
        {
            db.ProspectContacts.Add(new ProspectContact
            {
                Id = Guid.NewGuid(),
                ProspectId = prospectId,
                Name = "Test Contact",
                CreatedDate = createdDate,
                LastUpdatedDate = createdDate,
            });
            db.SaveChanges();
        }

        [Fact]
        public async Task Generate_EmptyDatabase_ReturnsZeroCounts()
        {
            var report = await sut.GenerateAsync(weekEnding);

            Assert.Equal(0, report.ProspectsResearched);
            Assert.Equal(0, report.NewContactsAdded);
            Assert.Equal(0, report.OutreachTouches);
            Assert.Equal(0, report.FollowUpTouches);
            Assert.Equal(0, report.Responses);
            Assert.Equal(0, report.MeetingsRequested);
            Assert.Equal(0, report.MeetingsScheduled);
            Assert.Equal(0, report.MeetingsHeld);
            Assert.Empty(report.KeyMunicipalFeedback);
            Assert.Empty(report.PricingFeedback);
        }

        [Fact]
        public async Task Generate_CountsProspectsCreatedInWindow_ExcludesOutside()
        {
            AddProspect(windowMid);
            AddProspect(windowStart);
            AddProspect(windowEnd);
            AddProspect(outsideWindow);

            var report = await sut.GenerateAsync(weekEnding);

            Assert.Equal(3, report.ProspectsResearched);
        }

        [Fact]
        public async Task Generate_CountsContactsCreatedInWindow()
        {
            var prospect = AddProspect(outsideWindow);
            AddContact(prospect.Id, windowMid);
            AddContact(prospect.Id, windowStart);
            AddContact(prospect.Id, outsideWindow);

            var report = await sut.GenerateAsync(weekEnding);

            Assert.Equal(2, report.NewContactsAdded);
        }

        [Fact]
        public async Task Generate_ActivityTypeMatchesAreCaseInsensitive()
        {
            var prospect = AddProspect(outsideWindow);
            AddActivity(prospect.Id, "Outreach", windowMid);
            AddActivity(prospect.Id, "OUTREACH", windowMid);
            AddActivity(prospect.Id, "outreach", windowMid);
            AddActivity(prospect.Id, "outreach", outsideWindow); // outside window — ignored

            var report = await sut.GenerateAsync(weekEnding);

            Assert.Equal(3, report.OutreachTouches);
        }

        [Fact]
        public async Task Generate_CountsEachActivityCategoryIndependently()
        {
            var prospect = AddProspect(outsideWindow);
            AddActivity(prospect.Id, "Outreach", windowMid);
            AddActivity(prospect.Id, "FollowUp", windowMid);
            AddActivity(prospect.Id, "FollowUp", windowMid);
            AddActivity(prospect.Id, "ResponseReceived", windowMid);
            AddActivity(prospect.Id, "MeetingRequested", windowMid);
            AddActivity(prospect.Id, "MeetingScheduled", windowMid);
            AddActivity(prospect.Id, "MeetingHeld", windowMid);
            AddActivity(prospect.Id, "EmailSent", windowMid); // legacy — unrecognised, not counted

            var report = await sut.GenerateAsync(weekEnding);

            Assert.Equal(1, report.OutreachTouches);
            Assert.Equal(2, report.FollowUpTouches);
            Assert.Equal(1, report.Responses);
            Assert.Equal(1, report.MeetingsRequested);
            Assert.Equal(1, report.MeetingsScheduled);
            Assert.Equal(1, report.MeetingsHeld);
        }

        [Fact]
        public async Task Generate_FeedbackAggregation_TouchedProspectsOnly_DedupedCaseInsensitive()
        {
            var touched = AddProspect(outsideWindow, keyObjection: "Budget cycle", pricingFeedback: "Prefer usage-based");
            var alsoTouched = AddProspect(outsideWindow, keyObjection: "budget cycle", pricingFeedback: "Council review");
            var untouched = AddProspect(outsideWindow, keyObjection: "Should not appear", pricingFeedback: "Should not appear");

            AddActivity(touched.Id, "Outreach", windowMid);
            AddActivity(alsoTouched.Id, "FollowUp", windowMid);
            // untouched has no activity in the window

            var report = await sut.GenerateAsync(weekEnding);

            Assert.Single(report.KeyMunicipalFeedback); // "Budget cycle" and "budget cycle" dedup case-insensitively
            Assert.Equal(2, report.PricingFeedback.Count); // "Prefer usage-based" and "Council review"
            Assert.DoesNotContain("Should not appear", report.KeyMunicipalFeedback);
            Assert.DoesNotContain("Should not appear", report.PricingFeedback);
        }

        [Fact]
        public async Task Generate_WindowBoundaries_AreInclusive()
        {
            AddProspect(new DateTimeOffset(2026, 7, 6, 0, 0, 0, TimeSpan.Zero)); // start-of-window, inclusive
            AddProspect(new DateTimeOffset(2026, 7, 12, 23, 59, 59, TimeSpan.Zero)); // end-of-window, inclusive
            AddProspect(new DateTimeOffset(2026, 7, 5, 23, 59, 59, TimeSpan.Zero)); // one second before start — excluded

            var report = await sut.GenerateAsync(weekEnding);

            Assert.Equal(2, report.ProspectsResearched);
            Assert.Equal(
                new DateTimeOffset(2026, 7, 6, 0, 0, 0, TimeSpan.Zero),
                report.PeriodStart);
            // PeriodEnd is inclusive-end-of-day (23:59:59.999)
            Assert.Equal(2026, report.PeriodEnd.Year);
            Assert.Equal(7, report.PeriodEnd.Month);
            Assert.Equal(12, report.PeriodEnd.Day);
        }

        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
