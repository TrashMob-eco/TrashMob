namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class ProspectActivityManagerTests
    {
        private readonly Mock<IKeyedRepository<ProspectActivity>> _activityRepo;
        private readonly Mock<IKeyedRepository<CommunityProspect>> _prospectRepo;
        private readonly Mock<ISentimentAnalysisService> _sentimentService;
        private readonly ProspectActivityManager _sut;
        private readonly Guid _userId = Guid.NewGuid();

        public ProspectActivityManagerTests()
        {
            _activityRepo = new Mock<IKeyedRepository<ProspectActivity>>();
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _sentimentService = new Mock<ISentimentAnalysisService>();

            _activityRepo.SetupAddAsync();

            _sut = new ProspectActivityManager(
                _activityRepo.Object,
                _prospectRepo.Object,
                _sentimentService.Object);
        }

        [Fact]
        public async Task AddAsync_WhenReplyActivity_CallsSentimentAnalysis()
        {
            var prospect = new CommunityProspectBuilder().WithPipelineStage(0).Build();
            _prospectRepo.SetupGetAsync(prospect);
            _prospectRepo.SetupUpdateAsync();
            _sentimentService
                .Setup(s => s.AnalyzeSentimentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Positive");

            var activity = CreateActivity(prospect.Id, "Reply", "Thanks for reaching out!");

            var result = await _sut.AddAsync(activity, _userId);

            Assert.Equal("Positive", result.SentimentScore);
            _sentimentService.Verify(
                s => s.AnalyzeSentimentAsync("Thanks for reaching out!", It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenReplyAndStageContacted_AdvancesToResponded()
        {
            var prospect = new CommunityProspectBuilder().WithPipelineStage(1).Build();
            _prospectRepo.SetupGetAsync(prospect);
            _prospectRepo.SetupUpdateAsync();
            _sentimentService
                .Setup(s => s.AnalyzeSentimentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Neutral");

            var activity = CreateActivity(prospect.Id, "Reply", "Got your email");

            await _sut.AddAsync(activity, _userId);

            Assert.Equal(2, prospect.PipelineStage);
            _prospectRepo.Verify(r => r.UpdateAsync(It.Is<CommunityProspect>(p => p.PipelineStage == 2)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenPositiveReplyAndStageResponded_AdvancesToInterested()
        {
            var prospect = new CommunityProspectBuilder().WithPipelineStage(2).Build();
            _prospectRepo.SetupGetAsync(prospect);
            _prospectRepo.SetupUpdateAsync();
            _sentimentService
                .Setup(s => s.AnalyzeSentimentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Positive");

            var activity = CreateActivity(prospect.Id, "Reply", "We would love to partner!");

            await _sut.AddAsync(activity, _userId);

            Assert.Equal(3, prospect.PipelineStage);
            _prospectRepo.Verify(r => r.UpdateAsync(It.Is<CommunityProspect>(p => p.PipelineStage == 3)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenNoteActivity_DoesNotCallSentimentAnalysis()
        {
            var activity = CreateActivity(Guid.NewGuid(), "Note", "Internal note");

            await _sut.AddAsync(activity, _userId);

            _sentimentService.Verify(
                s => s.AnalyzeSentimentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AddAsync_WhenNegativeReplyAndStageContacted_AdvancesToRespondedOnly()
        {
            var prospect = new CommunityProspectBuilder().WithPipelineStage(1).Build();
            _prospectRepo.SetupGetAsync(prospect);
            _prospectRepo.SetupUpdateAsync();
            _sentimentService
                .Setup(s => s.AnalyzeSentimentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Negative");

            var activity = CreateActivity(prospect.Id, "Reply", "Not interested");

            await _sut.AddAsync(activity, _userId);

            // Should advance from 1 to 2 (Contacted -> Responded), but NOT to 3
            Assert.Equal(2, prospect.PipelineStage);
        }

        private static ProspectActivity CreateActivity(Guid prospectId, string type, string details)
        {
            var userId = Guid.NewGuid();
            return new ProspectActivity
            {
                Id = Guid.NewGuid(),
                ProspectId = prospectId,
                ActivityType = type,
                Subject = "Test",
                Details = details,
                CreatedByUserId = userId,
                LastUpdatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }
    }
}
