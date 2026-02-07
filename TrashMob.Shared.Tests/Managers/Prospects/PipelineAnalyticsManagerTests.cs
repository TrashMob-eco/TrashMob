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
        private readonly PipelineAnalyticsManager _sut;

        public PipelineAnalyticsManagerTests()
        {
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _emailRepo = new Mock<IKeyedRepository<ProspectOutreachEmail>>();
            _sut = new PipelineAnalyticsManager(_prospectRepo.Object, _emailRepo.Object);
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
    }
}
