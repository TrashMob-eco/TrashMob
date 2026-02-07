namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class ProspectOutreachManagerTests
    {
        private readonly Mock<IKeyedRepository<CommunityProspect>> _prospectRepo;
        private readonly Mock<IKeyedRepository<ProspectOutreachEmail>> _outreachEmailRepo;
        private readonly Mock<IKeyedRepository<ProspectActivity>> _activityRepo;
        private readonly Mock<IOutreachContentService> _contentService;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<ProspectOutreachManager>> _logger;
        private readonly ProspectOutreachManager _sut;

        public ProspectOutreachManagerTests()
        {
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _outreachEmailRepo = new Mock<IKeyedRepository<ProspectOutreachEmail>>();
            _activityRepo = new Mock<IKeyedRepository<ProspectActivity>>();
            _contentService = new Mock<IOutreachContentService>();
            _emailManager = new Mock<IEmailManager>();
            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<ProspectOutreachManager>>();

            // Default: outreach enabled, test mode on
            _configuration.Setup(c => c["OutreachEnabled"]).Returns("true");
            _configuration.Setup(c => c["OutreachTestMode"]).Returns("true");
            _configuration.Setup(c => c["OutreachTestRecipientEmail"]).Returns("test@test.com");
            _configuration.Setup(c => c["OutreachMaxDailyOutreach"]).Returns("10");
            _configuration.Setup(c => c["OutreachMaxFollowUpsPerRun"]).Returns("10");

            _outreachEmailRepo.SetupAddAsync();
            _activityRepo.SetupAddAsync();
            _prospectRepo.SetupUpdateAsync();

            _sut = new ProspectOutreachManager(
                _prospectRepo.Object,
                _outreachEmailRepo.Object,
                _activityRepo.Object,
                _contentService.Object,
                _emailManager.Object,
                _configuration.Object,
                _logger.Object);
        }

        #region SendOutreachAsync

        [Fact]
        public async Task SendOutreach_WhenOutreachDisabled_ReturnsFailure()
        {
            _configuration.Setup(c => c["OutreachEnabled"]).Returns("false");
            var sut = CreateSut();

            var result = await sut.SendOutreachAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Contains("disabled", result.ErrorMessage);
        }

        [Fact]
        public async Task SendOutreach_WhenProspectNotFound_ReturnsFailure()
        {
            _prospectRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CommunityProspect)null);

            var result = await _sut.SendOutreachAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Contains("not found", result.ErrorMessage);
        }

        [Fact]
        public async Task SendOutreach_WhenProspectInStage5_ReturnsFailure()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.PipelineStage = 5;
            prospect.ContactEmail = "test@example.com";
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();

            var result = await _sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Contains("stage", result.ErrorMessage);
        }

        [Fact]
        public async Task SendOutreach_WhenNoContactEmail_ReturnsFailure()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = null;
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();

            // In test mode, test recipient email is used, so this won't fail for missing contact email.
            // Switch to live mode:
            _configuration.Setup(c => c["OutreachTestMode"]).Returns("false");
            var sut = CreateSut();

            var result = await sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Contains("no contact email", result.ErrorMessage);
        }

        [Fact]
        public async Task SendOutreach_WhenCadenceComplete_ReturnsFailure()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "test@example.com";
            _prospectRepo.SetupGetAsync(prospect);

            // Set up 4 existing outreach emails (cadence complete)
            var existingEmails = Enumerable.Range(1, 4).Select(step =>
                new ProspectOutreachEmailBuilder()
                    .WithProspectId(prospect.Id)
                    .WithCadenceStep(step)
                    .Build()).ToList();
            _outreachEmailRepo.SetupGetWithFilter(existingEmails);

            var result = await _sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Contains("cadence", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SendOutreach_SuccessfulSend_CreatesEmailAndActivity()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "test@example.com";
            prospect.PipelineStage = 0;
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();
            SetupContentService(prospect.Id, 1);

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            var userId = Guid.NewGuid();
            var result = await _sut.SendOutreachAsync(prospect.Id, userId);

            Assert.True(result.Success);
            _outreachEmailRepo.Verify(r => r.AddAsync(It.Is<ProspectOutreachEmail>(
                e => e.ProspectId == prospect.Id && e.CadenceStep == 1 && e.Status == "Sent")), Times.Once);
            _activityRepo.Verify(r => r.AddAsync(It.Is<ProspectActivity>(
                a => a.ProspectId == prospect.Id && a.ActivityType == "EmailSent")), Times.Once);
        }

        [Fact]
        public async Task SendOutreach_AdvancesPipelineStage_FromNewToContacted()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "test@example.com";
            prospect.PipelineStage = 0;
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();
            SetupContentService(prospect.Id, 1);

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            await _sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            _prospectRepo.Verify(r => r.UpdateAsync(It.Is<CommunityProspect>(
                p => p.Id == prospect.Id && p.PipelineStage == 1)), Times.Once);
        }

        [Fact]
        public async Task SendOutreach_TestMode_SendsToTestEmail()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "real@example.com";
            prospect.PipelineStage = 0;
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();
            SetupContentService(prospect.Id, 1);

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            await _sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.StartsWith("[TEST]")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<Shared.Poco.EmailAddress>>(r => r[0].Email == "test@test.com"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendOutreach_LiveMode_SendsToProspectEmail()
        {
            _configuration.Setup(c => c["OutreachTestMode"]).Returns("false");
            var sut = CreateSut();

            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "real@example.com";
            prospect.PipelineStage = 0;
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();
            SetupContentService(prospect.Id, 1);

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            await sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => !s.StartsWith("[TEST]")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<Shared.Poco.EmailAddress>>(r => r[0].Email == "real@example.com"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendOutreach_SetsNextFollowUpDate()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "test@example.com";
            prospect.PipelineStage = 0;
            _prospectRepo.SetupGetAsync(prospect);
            SetupEmptyOutreachHistory();
            SetupContentService(prospect.Id, 1);

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            await _sut.SendOutreachAsync(prospect.Id, Guid.NewGuid());

            _prospectRepo.Verify(r => r.UpdateAsync(It.Is<CommunityProspect>(
                p => p.NextFollowUpDate != null && p.LastContactedDate != null)), Times.Once);
        }

        #endregion

        #region PreviewOutreachAsync

        [Fact]
        public async Task PreviewOutreach_WhenProspectNotFound_ReturnsNotFoundMessage()
        {
            _prospectRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CommunityProspect)null);

            var result = await _sut.PreviewOutreachAsync(Guid.NewGuid());

            Assert.Contains("not found", result.Subject);
        }

        [Fact]
        public async Task PreviewOutreach_DeterminesCorrectCadenceStep()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "test@example.com";
            _prospectRepo.SetupGetAsync(prospect);

            // One existing email at step 1
            var existingEmails = new List<ProspectOutreachEmail>
            {
                new ProspectOutreachEmailBuilder().WithProspectId(prospect.Id).WithCadenceStep(1).Build()
            };
            _outreachEmailRepo.SetupGetWithFilter(existingEmails);

            SetupContentService(prospect.Id, 2);

            var result = await _sut.PreviewOutreachAsync(prospect.Id);

            Assert.Equal(2, result.CadenceStep);
        }

        #endregion

        #region BatchOutreachAsync

        [Fact]
        public async Task SendBatchOutreach_ProcessesMultipleProspects()
        {
            var prospect1 = new CommunityProspectBuilder().Build();
            prospect1.ContactEmail = "p1@example.com";
            var prospect2 = new CommunityProspectBuilder().Build();
            prospect2.ContactEmail = "p2@example.com";

            _prospectRepo.SetupGetAsync(new[] { prospect1, prospect2 });
            SetupEmptyOutreachHistory();
            _contentService.Setup(s => s.GenerateOutreachContentAsync(
                    It.IsAny<CommunityProspect>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CommunityProspect p, int step, int events, CancellationToken _) =>
                    new OutreachPreview { ProspectId = p.Id, CadenceStep = step, Subject = "Test", HtmlBody = "<p>Test</p>" });

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            var result = await _sut.SendBatchOutreachAsync(
                new List<Guid> { prospect1.Id, prospect2.Id }, Guid.NewGuid());

            Assert.Equal(2, result.TotalRequested);
            Assert.Equal(2, result.Sent);
        }

        #endregion

        #region ProcessDueFollowUpsAsync

        [Fact]
        public async Task ProcessDueFollowUps_WhenDisabled_ReturnsZero()
        {
            _configuration.Setup(c => c["OutreachEnabled"]).Returns("false");
            var sut = CreateSut();

            var count = await sut.ProcessDueFollowUpsAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task ProcessDueFollowUps_ProcessesDueProspects()
        {
            var prospect = new CommunityProspectBuilder().Build();
            prospect.ContactEmail = "test@example.com";
            prospect.PipelineStage = 1;
            prospect.NextFollowUpDate = DateTimeOffset.UtcNow.AddHours(-1);

            _prospectRepo.SetupGetWithFilter(new[] { prospect });
            _prospectRepo.SetupGetAsync(prospect);

            // One existing email at step 1
            var existingEmails = new List<ProspectOutreachEmail>
            {
                new ProspectOutreachEmailBuilder().WithProspectId(prospect.Id).WithCadenceStep(1).Build()
            };
            _outreachEmailRepo.SetupGetWithFilter(existingEmails);

            SetupContentService(prospect.Id, 2);
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{personalizedContent}</p>");

            var count = await _sut.ProcessDueFollowUpsAsync();

            Assert.Equal(1, count);
        }

        #endregion

        #region GetOutreachSettings

        [Fact]
        public void GetOutreachSettings_ReturnsConfiguredValues()
        {
            var settings = _sut.GetOutreachSettings();

            Assert.True(settings.OutreachEnabled);
            Assert.True(settings.TestMode);
            Assert.Equal("test@test.com", settings.TestRecipientEmail);
            Assert.Equal(10, settings.MaxDailyOutreach);
        }

        [Fact]
        public void GetOutreachSettings_DefaultsWhenNotConfigured()
        {
            _configuration.Setup(c => c["OutreachEnabled"]).Returns((string)null);
            _configuration.Setup(c => c["OutreachTestMode"]).Returns((string)null);
            _configuration.Setup(c => c["OutreachTestRecipientEmail"]).Returns((string)null);
            var sut = CreateSut();

            var settings = sut.GetOutreachSettings();

            Assert.False(settings.OutreachEnabled);
            Assert.True(settings.TestMode); // Default true
            Assert.Equal("info@trashmob.eco", settings.TestRecipientEmail);
        }

        #endregion

        #region Helpers

        private ProspectOutreachManager CreateSut()
        {
            return new ProspectOutreachManager(
                _prospectRepo.Object,
                _outreachEmailRepo.Object,
                _activityRepo.Object,
                _contentService.Object,
                _emailManager.Object,
                _configuration.Object,
                _logger.Object);
        }

        private void SetupEmptyOutreachHistory()
        {
            _outreachEmailRepo.SetupGetWithFilter(new List<ProspectOutreachEmail>());
        }

        private void SetupContentService(Guid prospectId, int expectedStep)
        {
            _contentService.Setup(s => s.GenerateOutreachContentAsync(
                    It.Is<CommunityProspect>(p => p.Id == prospectId),
                    expectedStep,
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OutreachPreview
                {
                    ProspectId = prospectId,
                    CadenceStep = expectedStep,
                    Subject = $"Test Subject Step {expectedStep}",
                    HtmlBody = $"<p>Test body step {expectedStep}</p>",
                    TokensUsed = 100,
                });
        }

        #endregion
    }
}
