namespace TrashMob.Shared.Tests.Managers.Partners
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Partners;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="PartnerRequestManager"/>.
    /// </summary>
    public class PartnerRequestManagerTests
    {
        private readonly Mock<IKeyedRepository<PartnerRequest>> _partnerRequestRepository;
        private readonly Mock<IKeyedManager<Partner>> _partnerManager;
        private readonly Mock<IBaseManager<PartnerAdmin>> _partnerUserManager;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly PartnerRequestManager _sut;

        public PartnerRequestManagerTests()
        {
            _partnerRequestRepository = new Mock<IKeyedRepository<PartnerRequest>>();
            _partnerManager = new Mock<IKeyedManager<Partner>>();
            _partnerUserManager = new Mock<IBaseManager<PartnerAdmin>>();
            _emailManager = new Mock<IEmailManager>();

            // Default setup for common operations
            _partnerRequestRepository.SetupAddAsync();
            _partnerRequestRepository.SetupUpdateAsync();
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new PartnerRequestManager(
                _partnerRequestRepository.Object,
                _partnerManager.Object,
                _partnerUserManager.Object,
                _emailManager.Object);
        }

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_BecomeAPartnerRequest_SetsStatusToPending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            // Act
            var result = await _sut.AddAsync(request, userId);

            // Assert
            Assert.Equal((int)PartnerRequestStatusEnum.Pending, result.PartnerRequestStatusId);
        }

        [Fact]
        public async Task AddAsync_SendPartnerInviteRequest_SetsStatusToSent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .AsSendPartnerInviteRequest()
                .CreatedBy(userId)
                .Build();

            // Act
            var result = await _sut.AddAsync(request, userId);

            // Assert
            Assert.Equal((int)PartnerRequestStatusEnum.Sent, result.PartnerRequestStatusId);
        }

        [Fact]
        public async Task AddAsync_BecomeAPartnerRequest_SendsEmailToAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .AsBecomeAPartnerRequest()
                .WithName("Test Org")
                .WithEmail("test@test.com")
                .WithNotes("Want to partner")
                .CreatedBy(userId)
                .Build();

            // Act
            await _sut.AddAsync(request, userId);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "Partner Request",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == Constants.TrashMobEmailAddress)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_BusinessPartnerInviteRequest_SendsEmailToPartner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .AsSendPartnerInviteRequest()
                .AsBusinessPartner()
                .WithName("Local Business")
                .WithEmail("business@test.com")
                .CreatedBy(userId)
                .Build();

            // Act
            await _sut.AddAsync(request, userId);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteBusinessPartner.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "Someone in your community wants you to become a TrashMob Partner!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "business@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_GovernmentPartnerInviteRequest_SendsEmailToPartner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .AsSendPartnerInviteRequest()
                .AsGovernmentPartner()
                .WithName("City Parks")
                .WithEmail("parks@city.gov")
                .CreatedBy(userId)
                .Build();

            // Act
            await _sut.AddAsync(request, userId);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteGovernmentPartner.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "Someone in your community wants your community to become a TrashMob Community!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "parks@city.gov")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_SendPartnerInviteRequest_AlsoNotifiesAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .AsSendPartnerInviteRequest()
                .AsBusinessPartner()
                .WithName("Local Business")
                .WithEmail("business@test.com")
                .CreatedBy(userId)
                .Build();

            // Act
            await _sut.AddAsync(request, userId);

            // Assert - Two emails: one to partner, one to admin
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        #endregion

        #region ApproveBecomeAPartnerAsync Tests

        [Fact]
        public async Task ApproveBecomeAPartnerAsync_SetsStatusToApproved()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);
            _partnerManager.Setup(m => m.AddAsync(It.IsAny<Partner>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner p, Guid _, CancellationToken _) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });
            _partnerUserManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pu, Guid _, CancellationToken _) => pu);

            // Act
            var result = await _sut.ApproveBecomeAPartnerAsync(requestId, userId, CancellationToken.None);

            // Assert
            Assert.Equal((int)PartnerRequestStatusEnum.Approved, result.PartnerRequestStatusId);
        }

        [Fact]
        public async Task ApproveBecomeAPartnerAsync_CreatesNewPartner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .WithName("New Partner Org")
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);
            _partnerManager.Setup(m => m.AddAsync(It.IsAny<Partner>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner p, Guid _, CancellationToken _) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });
            _partnerUserManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pu, Guid _, CancellationToken _) => pu);

            // Act
            await _sut.ApproveBecomeAPartnerAsync(requestId, userId, CancellationToken.None);

            // Assert
            _partnerManager.Verify(m => m.AddAsync(
                It.Is<Partner>(p => p.Name == "New Partner Org"),
                userId,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ApproveBecomeAPartnerAsync_CreatesPartnerAdmin()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(creatorId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);
            // The manager uses partnerRequest.CreatedByUserId for AddAsync
            _partnerManager.Setup(m => m.AddAsync(It.IsAny<Partner>(), creatorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner p, Guid _, CancellationToken _) =>
                {
                    p.Id = partnerId;
                    return p;
                });
            _partnerUserManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), creatorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pu, Guid _, CancellationToken _) => pu);

            // Act
            await _sut.ApproveBecomeAPartnerAsync(requestId, adminId, CancellationToken.None);

            // Assert
            _partnerUserManager.Verify(m => m.AddAsync(
                It.Is<PartnerAdmin>(pu => pu.PartnerId == partnerId && pu.UserId == creatorId),
                creatorId,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ApproveBecomeAPartnerAsync_SendsAcceptanceEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .WithName("Approved Org")
                .WithEmail("approved@test.com")
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);
            _partnerManager.Setup(m => m.AddAsync(It.IsAny<Partner>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner p, Guid _, CancellationToken _) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });
            _partnerUserManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pu, Guid _, CancellationToken _) => pu);

            // Act
            await _sut.ApproveBecomeAPartnerAsync(requestId, userId, CancellationToken.None);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.PartnerRequestAccepted.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "Your request to become a TrashMob.eco Partner has been accepted!",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "approved@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DenyBecomeAPartnerAsync Tests

        [Fact]
        public async Task DenyBecomeAPartnerAsync_SetsStatusToDenied()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);

            // Act
            var result = await _sut.DenyBecomeAPartnerAsync(requestId, userId, CancellationToken.None);

            // Assert
            Assert.Equal((int)PartnerRequestStatusEnum.Denied, result.PartnerRequestStatusId);
        }

        [Fact]
        public async Task DenyBecomeAPartnerAsync_DoesNotCreatePartner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);

            // Act
            await _sut.DenyBecomeAPartnerAsync(requestId, userId, CancellationToken.None);

            // Assert
            _partnerManager.Verify(m => m.AddAsync(It.IsAny<Partner>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DenyBecomeAPartnerAsync_SendsDeclinedEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new PartnerRequestBuilder()
                .WithId(requestId)
                .WithName("Denied Org")
                .WithEmail("denied@test.com")
                .AsPending()
                .AsBecomeAPartnerRequest()
                .CreatedBy(userId)
                .Build();

            _partnerRequestRepository.SetupGetAsync(request);

            // Act
            await _sut.DenyBecomeAPartnerAsync(requestId, userId, CancellationToken.None);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.PartnerRequestDeclined.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                "Your request to become a TrashMob.eco Partner has been declined",
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "denied@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
