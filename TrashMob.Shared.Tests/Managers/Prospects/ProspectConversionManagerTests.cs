namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
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

    public class ProspectConversionManagerTests
    {
        private readonly Mock<IKeyedRepository<CommunityProspect>> _prospectRepo;
        private readonly Mock<IKeyedManager<Partner>> _partnerManager;
        private readonly Mock<IBaseManager<PartnerAdmin>> _partnerAdminManager;
        private readonly Mock<IKeyedRepository<ProspectActivity>> _activityRepo;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly Mock<ILogger<ProspectConversionManager>> _logger;
        private readonly ProspectConversionManager _sut;
        private readonly Guid _userId = Guid.NewGuid();

        public ProspectConversionManagerTests()
        {
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _partnerManager = new Mock<IKeyedManager<Partner>>();
            _partnerAdminManager = new Mock<IBaseManager<PartnerAdmin>>();
            _activityRepo = new Mock<IKeyedRepository<ProspectActivity>>();
            _emailManager = new Mock<IEmailManager>();
            _logger = new Mock<ILogger<ProspectConversionManager>>();

            _prospectRepo.SetupUpdateAsync();
            _activityRepo.SetupAddAsync();

            _sut = new ProspectConversionManager(
                _prospectRepo.Object,
                _partnerManager.Object,
                _partnerAdminManager.Object,
                _activityRepo.Object,
                _emailManager.Object,
                _logger.Object);
        }

        [Fact]
        public async Task ConvertToPartner_CreatesPartnerAndUpdatesProspect()
        {
            var prospect = CreateProspect();
            SetupForConversion(prospect);

            var result = await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id },
                _userId);

            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.PartnerId);
        }

        [Fact]
        public async Task ConvertToPartner_CreatesPartnerAdmin()
        {
            var prospect = CreateProspect();
            SetupForConversion(prospect);

            await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id },
                _userId);

            _partnerAdminManager.Verify(
                m => m.AddAsync(
                    It.Is<PartnerAdmin>(pa => pa.UserId == _userId),
                    _userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ConvertToPartner_SetsConvertedPartnerId()
        {
            var prospect = CreateProspect();
            SetupForConversion(prospect);

            await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id },
                _userId);

            Assert.NotNull(prospect.ConvertedPartnerId);
            Assert.Equal(5, prospect.PipelineStage);
            _prospectRepo.Verify(r => r.UpdateAsync(It.Is<CommunityProspect>(
                p => p.ConvertedPartnerId.HasValue && p.PipelineStage == 5)), Times.Once);
        }

        [Fact]
        public async Task ConvertToPartner_WhenProspectNotFound_ReturnsFailure()
        {
            _prospectRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CommunityProspect)null);

            var result = await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = Guid.NewGuid() },
                _userId);

            Assert.False(result.Success);
            Assert.Contains("not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ConvertToPartner_WhenAlreadyConverted_ReturnsFailure()
        {
            var prospect = CreateProspect();
            prospect.ConvertedPartnerId = Guid.NewGuid();
            _prospectRepo.SetupGetAsync(prospect);

            var result = await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id },
                _userId);

            Assert.False(result.Success);
            Assert.Contains("already been converted", result.ErrorMessage);
        }

        [Fact]
        public async Task ConvertToPartner_LogsStatusChangeActivity()
        {
            var prospect = CreateProspect();
            SetupForConversion(prospect);

            await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id },
                _userId);

            _activityRepo.Verify(
                r => r.AddAsync(It.Is<ProspectActivity>(
                    a => a.ActivityType == "StatusChange" && a.Subject == "Converted to Partner")),
                Times.Once);
        }

        [Fact]
        public async Task ConvertToPartner_SendsWelcomeEmail_WhenRequested()
        {
            var prospect = CreateProspect();
            SetupForConversion(prospect);
            _emailManager.Setup(m => m.GetHtmlEmailCopy(It.IsAny<string>())).Returns("<p>Welcome {CommunityName}</p>");

            await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id, SendWelcomeEmail = true },
                _userId);

            _emailManager.Verify(
                m => m.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<Shared.Poco.EmailAddress>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ConvertToPartner_SkipsWelcomeEmail_WhenNotRequested()
        {
            var prospect = CreateProspect();
            SetupForConversion(prospect);

            await _sut.ConvertToPartnerAsync(
                new ProspectConversionRequest { ProspectId = prospect.Id, SendWelcomeEmail = false },
                _userId);

            _emailManager.Verify(
                m => m.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<Shared.Poco.EmailAddress>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static CommunityProspect CreateProspect()
        {
            return new CommunityProspectBuilder()
                .WithPipelineStage(3)
                .WithContactInfo("test@city.gov", "John Doe", "Director")
                .WithWebsite("https://city.gov")
                .Build();
        }

        private void SetupForConversion(CommunityProspect prospect)
        {
            _prospectRepo.SetupGetAsync(prospect);

            _partnerManager.Setup(m => m.AddAsync(It.IsAny<Partner>(), _userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner p, Guid _, CancellationToken _) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });

            _partnerAdminManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), _userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pa, Guid _, CancellationToken _) => pa);
        }
    }
}
