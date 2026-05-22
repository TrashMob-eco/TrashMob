namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class PipelineAnalyticsManagerTests
    {
        private readonly Mock<IKeyedRepository<CommunityProspect>> _prospectRepo;
        private readonly Mock<IKeyedRepository<ProspectOutreachEmail>> _emailRepo;
        private readonly Mock<IKeyedRepository<ProspectActivity>> _activityRepo;
        private readonly Mock<IKeyedRepository<User>> _userRepo;
        private readonly PipelineAnalyticsManager _sut;

        public PipelineAnalyticsManagerTests()
        {
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _emailRepo = new Mock<IKeyedRepository<ProspectOutreachEmail>>();
            _activityRepo = new Mock<IKeyedRepository<ProspectActivity>>();
            _userRepo = new Mock<IKeyedRepository<User>>();

            // Default empty data for the touchpoint tally so existing tests don't have to wire it.
            _activityRepo.SetupGet(new List<ProspectActivity>());
            _userRepo.SetupGet(new List<User>());

            _sut = new PipelineAnalyticsManager(
                _prospectRepo.Object,
                _emailRepo.Object,
                _activityRepo.Object,
                _userRepo.Object);
        }

        [Fact]
        public async Task GetAnalytics_ReturnsCorrectStageCounts()
        {
            var prospects = new List<CommunityProspect>
            {
                new CommunityProspectBuilder().WithPipelineStage(0).Build(),
                new CommunityProspectBuilder().WithPipelineStage(0).Build(),
                new CommunityProspectBuilder().WithPipelineStage(1).Build(),
                new CommunityProspectBuilder().WithPipelineStage(3).Build(),
            };
            _prospectRepo.SetupGet(prospects);
            _emailRepo.SetupGet(new List<ProspectOutreachEmail>());

            var result = await _sut.GetAnalyticsAsync();

            Assert.Equal(4, result.TotalProspects);
            Assert.Equal(2, result.StageCounts.First(s => s.Stage == 0).Count);
            Assert.Equal(1, result.StageCounts.First(s => s.Stage == 1).Count);
            Assert.Equal(0, result.StageCounts.First(s => s.Stage == 2).Count);
            Assert.Equal(1, result.StageCounts.First(s => s.Stage == 3).Count);
        }

        [Fact]
        public async Task GetAnalytics_ComputesOutreachMetrics()
        {
            _prospectRepo.SetupGet(new List<CommunityProspect>());

            var emails = new List<ProspectOutreachEmail>
            {
                CreateEmail("Sent"),
                CreateEmail("Delivered"),
                CreateEmail("Opened"),
                CreateEmail("Clicked"),
                CreateEmail("Bounced"),
                CreateEmail("Failed"),
            };
            _emailRepo.SetupGet(emails);

            var result = await _sut.GetAnalyticsAsync();

            // Sent includes: Sent, Delivered, Opened, Clicked = 4
            Assert.Equal(4, result.TotalEmailsSent);
            // Opened includes: Opened, Clicked = 2
            Assert.Equal(2, result.TotalEmailsOpened);
            Assert.Equal(1, result.TotalEmailsClicked);
            Assert.Equal(1, result.TotalEmailsBounced);
            Assert.Equal(50.0, result.OpenRate);
            Assert.Equal(25.0, result.ClickRate);
            Assert.Equal(25.0, result.BounceRate);
        }

        [Fact]
        public async Task GetAnalytics_ComputesConversionRate()
        {
            var partnerId = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;

            var prospects = new List<CommunityProspect>
            {
                new CommunityProspectBuilder().WithPipelineStage(0).Build(),
                new CommunityProspectBuilder().WithPipelineStage(1).Build(),
                BuildConverted(partnerId, now.AddDays(-10), now),
                BuildConverted(Guid.NewGuid(), now.AddDays(-20), now),
            };
            _prospectRepo.SetupGet(prospects);
            _emailRepo.SetupGet(new List<ProspectOutreachEmail>());

            var result = await _sut.GetAnalyticsAsync();

            Assert.Equal(2, result.ConvertedCount);
            Assert.Equal(50.0, result.ConversionRate);
            Assert.True(result.AverageDaysInPipeline > 0);
        }

        [Fact]
        public async Task GetAnalytics_WhenNoProspects_ReturnsZeroes()
        {
            _prospectRepo.SetupGet(new List<CommunityProspect>());
            _emailRepo.SetupGet(new List<ProspectOutreachEmail>());

            var result = await _sut.GetAnalyticsAsync();

            Assert.Equal(0, result.TotalProspects);
            Assert.Equal(0, result.ConvertedCount);
            Assert.Equal(0, result.ConversionRate);
            Assert.Equal(0, result.AverageDaysInPipeline);
            Assert.Equal(7, result.StageCounts.Count);
            Assert.All(result.StageCounts, s => Assert.Equal(0, s.Count));
        }

        private static ProspectOutreachEmail CreateEmail(string status)
        {
            var userId = Guid.NewGuid();
            return new ProspectOutreachEmail
            {
                Id = Guid.NewGuid(),
                ProspectId = Guid.NewGuid(),
                CadenceStep = 1,
                Subject = "Test",
                HtmlBody = "<p>Test</p>",
                Status = status,
                CreatedByUserId = userId,
                LastUpdatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }

        private static CommunityProspect BuildConverted(Guid partnerId, DateTimeOffset created, DateTimeOffset updated)
        {
            var prospect = new CommunityProspectBuilder()
                .WithPipelineStage(5)
                .Build();
            prospect.ConvertedPartnerId = partnerId;
            prospect.CreatedDate = created;
            prospect.LastUpdatedDate = updated;
            return prospect;
        }

        // ----- Project 60 Phase 4: per-user touchpoint tally -----

        [Fact]
        public async Task GetAnalytics_TouchpointTally_GroupsActivitiesAndOutreachByUser()
        {
            _prospectRepo.SetupGet(new List<CommunityProspect>());

            var now = DateTimeOffset.UtcNow;
            var alice = new User { Id = Guid.NewGuid(), UserName = "alice", GivenName = "Alice", Surname = "Smith" };
            var bob = new User { Id = Guid.NewGuid(), UserName = "bob" };
            _userRepo.SetupGet(new[] { alice, bob });

            _activityRepo.SetupGet(new[]
            {
                MakeActivity(alice.Id, now.AddDays(-5)),
                MakeActivity(alice.Id, now.AddDays(-10)),
                MakeActivity(bob.Id, now.AddDays(-20)),
            });

            _emailRepo.SetupGet(new[]
            {
                CreateEmail("Sent", alice.Id, now.AddDays(-3)),
                CreateEmail("Sent", bob.Id, now.AddDays(-15)),
                CreateEmail("Sent", bob.Id, now.AddDays(-25)),
            });

            var result = await _sut.GetAnalyticsAsync();

            var aliceStat = result.TouchpointsByUserLast30Days.Single(s => s.UserId == alice.Id);
            Assert.Equal("Alice Smith", aliceStat.UserName);
            Assert.Equal(2, aliceStat.ActivityCount);
            Assert.Equal(1, aliceStat.OutreachEmailCount);
            Assert.Equal(3, aliceStat.TotalTouchpoints);

            var bobStat = result.TouchpointsByUserLast30Days.Single(s => s.UserId == bob.Id);
            Assert.Equal("bob", bobStat.UserName);
            Assert.Equal(1, bobStat.ActivityCount);
            Assert.Equal(2, bobStat.OutreachEmailCount);
            Assert.Equal(3, bobStat.TotalTouchpoints);

            // Ordered by TotalTouchpoints descending (tie is fine — both have 3 here).
            Assert.Equal(2, result.TouchpointsByUserLast30Days.Count);
        }

        [Fact]
        public async Task GetAnalytics_TouchpointTally_ExcludesOldEntriesFromWindow()
        {
            _prospectRepo.SetupGet(new List<CommunityProspect>());

            var now = DateTimeOffset.UtcNow;
            var alice = new User { Id = Guid.NewGuid(), UserName = "alice" };
            _userRepo.SetupGet(new[] { alice });

            _activityRepo.SetupGet(new[]
            {
                MakeActivity(alice.Id, now.AddDays(-15)),
                MakeActivity(alice.Id, now.AddDays(-45)), // Outside the 30-day window
                MakeActivity(alice.Id, now.AddDays(-200)), // Outside the 90-day window
            });
            _emailRepo.SetupGet(new List<ProspectOutreachEmail>());

            var result = await _sut.GetAnalyticsAsync();

            var alice30 = result.TouchpointsByUserLast30Days.Single(s => s.UserId == alice.Id);
            Assert.Equal(1, alice30.TotalTouchpoints);

            var alice90 = result.TouchpointsByUserLast90Days.Single(s => s.UserId == alice.Id);
            Assert.Equal(2, alice90.TotalTouchpoints);
        }

        [Fact]
        public async Task GetAnalytics_TouchpointTally_HandlesUnknownUsers()
        {
            _prospectRepo.SetupGet(new List<CommunityProspect>());
            _userRepo.SetupGet(new List<User>());

            var orphanUserId = Guid.NewGuid();
            _activityRepo.SetupGet(new[] { MakeActivity(orphanUserId, DateTimeOffset.UtcNow.AddDays(-5)) });
            _emailRepo.SetupGet(new List<ProspectOutreachEmail>());

            var result = await _sut.GetAnalyticsAsync();

            var stat = Assert.Single(result.TouchpointsByUserLast30Days);
            Assert.Equal("(unknown)", stat.UserName);
            Assert.Equal(1, stat.TotalTouchpoints);
        }

        [Fact]
        public async Task GetAnalytics_TouchpointTally_IgnoresGuidEmptyUserIds()
        {
            _prospectRepo.SetupGet(new List<CommunityProspect>());
            _userRepo.SetupGet(new List<User>());

            // Automated/system-attributed entries (e.g. follow-up engine) carry Guid.Empty
            // and should not appear in the per-user report.
            _activityRepo.SetupGet(new[] { MakeActivity(Guid.Empty, DateTimeOffset.UtcNow.AddDays(-2)) });
            _emailRepo.SetupGet(new[] { CreateEmail("Sent", Guid.Empty, DateTimeOffset.UtcNow.AddDays(-2)) });

            var result = await _sut.GetAnalyticsAsync();

            Assert.Empty(result.TouchpointsByUserLast30Days);
        }

        private static ProspectActivity MakeActivity(Guid userId, DateTimeOffset createdDate)
        {
            return new ProspectActivity
            {
                Id = Guid.NewGuid(),
                ProspectId = Guid.NewGuid(),
                ActivityType = "Call",
                CreatedByUserId = userId,
                CreatedDate = createdDate,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = createdDate,
            };
        }

        private static ProspectOutreachEmail CreateEmail(string status, Guid userId, DateTimeOffset createdDate)
        {
            return new ProspectOutreachEmail
            {
                Id = Guid.NewGuid(),
                ProspectId = Guid.NewGuid(),
                CadenceStep = 1,
                Subject = "Test",
                HtmlBody = "<p>Test</p>",
                Status = status,
                CreatedByUserId = userId,
                LastUpdatedByUserId = userId,
                CreatedDate = createdDate,
                LastUpdatedDate = createdDate,
            };
        }
    }
}
