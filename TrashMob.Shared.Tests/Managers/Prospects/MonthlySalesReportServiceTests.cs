namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="MonthlySalesReportService"/> — Project 63 Phase 3.
    /// Covers default-target fallback, override behavior, status thresholds,
    /// market intelligence aggregation, and window boundaries.
    /// </summary>
    public class MonthlySalesReportServiceTests : IDisposable
    {
        private readonly MobDbContext db;
        private readonly MonthlySalesReportService sut;
        private readonly DateOnly reportMonth = new DateOnly(2026, 7, 15); // July 2026
        private readonly DateTimeOffset inMonthEarly = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        private readonly DateTimeOffset inMonthLate = new DateTimeOffset(2026, 7, 31, 23, 0, 0, TimeSpan.Zero);
        private readonly DateTimeOffset outOfMonthBefore = new DateTimeOffset(2026, 6, 30, 23, 0, 0, TimeSpan.Zero);
        private readonly DateTimeOffset outOfMonthAfter = new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);

        public MonthlySalesReportServiceTests()
        {
            var options = new DbContextOptionsBuilder<MobDbContext>()
                .UseInMemoryDatabase(databaseName: $"MonthlySalesReport_{Guid.NewGuid()}")
                .Options;
            db = new MobDbContext(options);
            sut = new MonthlySalesReportService(db);
        }

        private CommunityProspect AddProspect(
            DateTimeOffset createdDate,
            string department = null,
            string keyObjection = null,
            string pricingFeedback = null)
        {
            var prospect = new CommunityProspect
            {
                Id = Guid.NewGuid(),
                Name = "Test City",
                CreatedDate = createdDate,
                LastUpdatedDate = createdDate,
                Department = department,
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

        [Fact]
        public async Task Generate_EmptyDatabase_UsesDefaultTargets()
        {
            var report = await sut.GenerateAsync(reportMonth);

            Assert.Equal(7, report.Metrics.Count);
            Assert.Equal(20, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.ProspectsResearched).Target);
            Assert.Equal(20, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.NewContacts).Target);
            Assert.Equal(15, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.OutreachTouches).Target);
            Assert.Equal(10, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.FollowUpTouches).Target);
            Assert.Equal(3, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.Responses).Target);
            Assert.Equal(2, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.MeetingsRequested).Target);
            Assert.Equal(1, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.MeetingsScheduled).Target);
        }

        [Fact]
        public async Task Generate_EmptyDatabase_AllMetricsShowZeroActualAndBehindStatus()
        {
            var report = await sut.GenerateAsync(reportMonth);

            Assert.All(report.Metrics, m =>
            {
                Assert.Equal(0, m.Actual);
                Assert.Equal("Behind", m.Status);
            });
        }

        [Fact]
        public async Task Generate_ProspectAndContactCounts_RespectMonthBoundaries()
        {
            AddProspect(inMonthEarly);
            AddProspect(inMonthLate);
            AddProspect(outOfMonthBefore);
            AddProspect(outOfMonthAfter);
            db.ProspectContacts.Add(new ProspectContact
            {
                Id = Guid.NewGuid(),
                ProspectId = Guid.NewGuid(),
                Name = "Contact",
                CreatedDate = inMonthEarly,
                LastUpdatedDate = inMonthEarly,
            });
            db.ProspectContacts.Add(new ProspectContact
            {
                Id = Guid.NewGuid(),
                ProspectId = Guid.NewGuid(),
                Name = "Contact",
                CreatedDate = outOfMonthBefore,
                LastUpdatedDate = outOfMonthBefore,
            });
            db.SaveChanges();

            var report = await sut.GenerateAsync(reportMonth);

            Assert.Equal(2, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.ProspectsResearched).Actual);
            Assert.Equal(1, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.NewContacts).Actual);
        }

        [Fact]
        public async Task Generate_ActivityCategoriesAreCountedIndependently()
        {
            var prospect = AddProspect(outOfMonthBefore);
            AddActivity(prospect.Id, "Outreach", inMonthEarly);
            AddActivity(prospect.Id, "Outreach", inMonthEarly);
            AddActivity(prospect.Id, "FollowUp", inMonthEarly);
            AddActivity(prospect.Id, "ResponseReceived", inMonthEarly);
            AddActivity(prospect.Id, "MeetingRequested", inMonthEarly);
            AddActivity(prospect.Id, "MeetingScheduled", inMonthEarly);
            AddActivity(prospect.Id, "Outreach", outOfMonthAfter); // outside — ignored

            var report = await sut.GenerateAsync(reportMonth);

            Assert.Equal(2, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.OutreachTouches).Actual);
            Assert.Equal(1, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.FollowUpTouches).Actual);
            Assert.Equal(1, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.Responses).Actual);
            Assert.Equal(1, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.MeetingsRequested).Actual);
            Assert.Equal(1, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.MeetingsScheduled).Actual);
        }

        [Fact]
        public async Task Generate_StatusThresholds_BehindOnTrackExceeded()
        {
            var prospect = AddProspect(outOfMonthBefore);
            // Target 15 (OutreachTouches). Add 10 → 10/15 = 0.67 → Behind.
            for (var i = 0; i < 10; i++)
            {
                AddActivity(prospect.Id, "Outreach", inMonthEarly);
            }

            // Target 10 (FollowUpTouches). Add 8 → 8/10 = 0.80 → OnTrack.
            for (var i = 0; i < 8; i++)
            {
                AddActivity(prospect.Id, "FollowUp", inMonthEarly);
            }

            // Target 3 (Responses). Add 5 → 5/3 = 1.67 → Exceeded.
            for (var i = 0; i < 5; i++)
            {
                AddActivity(prospect.Id, "ResponseReceived", inMonthEarly);
            }

            var report = await sut.GenerateAsync(reportMonth);

            Assert.Equal("Behind", report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.OutreachTouches).Status);
            Assert.Equal("OnTrack", report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.FollowUpTouches).Status);
            Assert.Equal("Exceeded", report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.Responses).Status);
        }

        [Fact]
        public async Task Generate_StoredTarget_OverridesDefault()
        {
            var actor = Guid.NewGuid();
            await sut.UpdateTargetsAsync(reportMonth, new[]
            {
                new MonthlyTargetUpdateDto { Metric = (int)SalesMetricEnum.OutreachTouches, Target = 50 },
            }, actor);

            var report = await sut.GenerateAsync(reportMonth);

            var row = report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.OutreachTouches);
            Assert.Equal(50, row.Target);
            // Other metrics still fall back to Cynthia's defaults.
            Assert.Equal(20, report.Metrics.Single(m => m.Metric == (int)SalesMetricEnum.ProspectsResearched).Target);
        }

        [Fact]
        public async Task UpdateTargets_IsIdempotent_ExistingRowsAreUpdatedNotDuplicated()
        {
            var actor = Guid.NewGuid();
            await sut.UpdateTargetsAsync(reportMonth, new[]
            {
                new MonthlyTargetUpdateDto { Metric = (int)SalesMetricEnum.OutreachTouches, Target = 50 },
            }, actor);
            await sut.UpdateTargetsAsync(reportMonth, new[]
            {
                new MonthlyTargetUpdateDto { Metric = (int)SalesMetricEnum.OutreachTouches, Target = 60 },
            }, actor);

            var rows = await db.SalesMonthlyTargets
                .Where(t => t.Metric == (int)SalesMetricEnum.OutreachTouches)
                .ToListAsync();

            Assert.Single(rows);
            Assert.Equal(60, rows[0].Target);
        }

        [Fact]
        public async Task Generate_MarketIntelligence_BestRespondingDepartmentsCountsOnlyRespondedProspects()
        {
            var responder = AddProspect(outOfMonthBefore, department: "Public Works");
            var alsoResponder = AddProspect(outOfMonthBefore, department: "Public Works");
            var justTouched = AddProspect(outOfMonthBefore, department: "Sustainability");
            var noDepartment = AddProspect(outOfMonthBefore, department: null);

            AddActivity(responder.Id, "ResponseReceived", inMonthEarly);
            AddActivity(alsoResponder.Id, "ResponseReceived", inMonthEarly);
            AddActivity(justTouched.Id, "Outreach", inMonthEarly); // touched but did not respond
            AddActivity(noDepartment.Id, "ResponseReceived", inMonthEarly);

            var report = await sut.GenerateAsync(reportMonth);

            Assert.Single(report.BestRespondingDepartments);
            var pw = report.BestRespondingDepartments.First();
            Assert.Equal("Public Works", pw.Label);
            Assert.Equal(2, pw.Count);
        }

        [Fact]
        public async Task Generate_MarketIntelligence_ObjectionsAndPricingAreDedupedAndOrdered()
        {
            var p1 = AddProspect(outOfMonthBefore, keyObjection: "Budget cycle", pricingFeedback: "Prefer usage-based");
            var p2 = AddProspect(outOfMonthBefore, keyObjection: "budget cycle", pricingFeedback: "Council review");
            var p3 = AddProspect(outOfMonthBefore, keyObjection: "Waivers concern", pricingFeedback: "Prefer usage-based");
            AddActivity(p1.Id, "Outreach", inMonthEarly);
            AddActivity(p2.Id, "Outreach", inMonthEarly);
            AddActivity(p3.Id, "Outreach", inMonthEarly);

            var report = await sut.GenerateAsync(reportMonth);

            Assert.Equal(2, report.CommonObjections.Count); // "Budget cycle" and "Waivers concern"
            Assert.Equal("Budget cycle", report.CommonObjections[0].Label);
            Assert.Equal(2, report.CommonObjections[0].Count);
            Assert.Equal(1, report.CommonObjections[1].Count);

            Assert.Equal(2, report.PricingFeedback.Count);
            Assert.Equal("Prefer usage-based", report.PricingFeedback[0].Label);
            Assert.Equal(2, report.PricingFeedback[0].Count);
        }

        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
