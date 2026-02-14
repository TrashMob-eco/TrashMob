namespace TrashMob.Shared.Tests.Managers.Partners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    /// Unit tests for <see cref="PartnerAdminInvitationManager"/>.
    /// </summary>
    public class PartnerAdminInvitationManagerTests
    {
        private readonly Mock<IKeyedRepository<PartnerAdminInvitation>> _invitationRepository;
        private readonly Mock<IPartnerAdminManager> _partnerAdminManager;
        private readonly Mock<IUserManager> _userManager;
        private readonly Mock<IKeyedManager<Partner>> _partnerManager;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly PartnerAdminInvitationManager _sut;

        public PartnerAdminInvitationManagerTests()
        {
            _invitationRepository = new Mock<IKeyedRepository<PartnerAdminInvitation>>();
            _partnerAdminManager = new Mock<IPartnerAdminManager>();
            _userManager = new Mock<IUserManager>();
            _partnerManager = new Mock<IKeyedManager<Partner>>();
            _emailManager = new Mock<IEmailManager>();

            // Default setup for common operations
            _invitationRepository.SetupAddAsync();
            _invitationRepository.SetupUpdateAsync();
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new PartnerAdminInvitationManager(
                _invitationRepository.Object,
                _partnerAdminManager.Object,
                _userManager.Object,
                _partnerManager.Object,
                _emailManager.Object);
        }

        #region GetByParentIdAsync Tests

        [Fact]
        public async Task GetByParentIdAsync_ReturnsInvitationsForPartner()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var invitation1 = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("admin1@test.com")
                .Build();
            var invitation2 = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("admin2@test.com")
                .Build();
            var otherInvitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(Guid.NewGuid())
                .Build();

            var allInvitations = new List<PartnerAdminInvitation> { invitation1, invitation2, otherInvitation };
            _invitationRepository.SetupGet(allInvitations);

            // Act
            var result = await _sut.GetByParentIdAsync(partnerId, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, i => Assert.Equal(partnerId, i.PartnerId));
        }

        [Fact]
        public async Task GetByParentIdAsync_ReturnsEmptyWhenNoInvitations()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var otherInvitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(Guid.NewGuid())
                .Build();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { otherInvitation });

            // Act
            var result = await _sut.GetByParentIdAsync(partnerId, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region AcceptInvitation Tests

        [Fact]
        public async Task AcceptInvitation_SetsStatusToAccepted()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithId(invitationId)
                .WithPartnerId(partnerId)
                .AsSent()
                .Build();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { invitation });
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation> { invitation });
            _partnerAdminManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pa, CancellationToken _) => pa);

            // Act
            await _sut.AcceptInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _invitationRepository.Verify(r => r.UpdateAsync(It.Is<PartnerAdminInvitation>(
                i => i.InvitationStatusId == (int)InvitationStatusEnum.Accepted)), Times.Once);
        }

        [Fact]
        public async Task AcceptInvitation_CreatesPartnerAdmin()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithId(invitationId)
                .WithPartnerId(partnerId)
                .AsSent()
                .Build();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { invitation });
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation> { invitation });
            _partnerAdminManager.Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerAdmin pa, CancellationToken _) => pa);

            // Act
            await _sut.AcceptInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _partnerAdminManager.Verify(m => m.AddAsync(
                It.Is<PartnerAdmin>(pa => pa.PartnerId == partnerId && pa.UserId == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AcceptInvitation_DoesNothingWhenInvitationNotFound()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation>());
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation>());

            // Act
            await _sut.AcceptInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _partnerAdminManager.Verify(m => m.AddAsync(It.IsAny<PartnerAdmin>(), It.IsAny<CancellationToken>()), Times.Never);
            _invitationRepository.Verify(r => r.UpdateAsync(It.IsAny<PartnerAdminInvitation>()), Times.Never);
        }

        #endregion

        #region DeclineInvitation Tests

        [Fact]
        public async Task DeclineInvitation_SetsStatusToDeclined()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithId(invitationId)
                .AsSent()
                .Build();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { invitation });
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation> { invitation });

            // Act
            await _sut.DeclineInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _invitationRepository.Verify(r => r.UpdateAsync(It.Is<PartnerAdminInvitation>(
                i => i.InvitationStatusId == (int)InvitationStatusEnum.Declined)), Times.Once);
        }

        [Fact]
        public async Task DeclineInvitation_DoesNotCreatePartnerAdmin()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithId(invitationId)
                .AsSent()
                .Build();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { invitation });
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation> { invitation });

            // Act
            await _sut.DeclineInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _partnerAdminManager.Verify(m => m.AddAsync(It.IsAny<PartnerAdmin>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeclineInvitation_DoesNothingWhenInvitationNotFound()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation>());
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation>());

            // Act
            await _sut.DeclineInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _invitationRepository.Verify(r => r.UpdateAsync(It.IsAny<PartnerAdminInvitation>()), Times.Never);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_NewUser_SetsStatusToNew_ThenSent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).WithName("Test Partner").Build();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("newuser@test.com")
                .Build();

            // No existing invitation
            _invitationRepository.SetupGet(new List<PartnerAdminInvitation>());
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation>());

            // User doesn't exist
            _userManager.Setup(m => m.GetUserByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _sut.AddAsync(invitation, userId, CancellationToken.None);

            // Assert - Email should be sent to new user
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteNewUserToBePartnerAdmin.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("Administrator")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "newuser@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ExistingUser_SendsExistingUserEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).WithName("Test Partner").Build();
            var existingUser = new UserBuilder().WithEmail("existing@test.com").WithUserName("ExistingUser").Build();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("existing@test.com")
                .Build();

            // No existing invitation
            _invitationRepository.SetupGet(new List<PartnerAdminInvitation>());
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation>());

            // User exists
            _userManager.Setup(m => m.GetUserByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // User is not already an admin
            _partnerAdminManager.Setup(m => m.GetPartnersByUserIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Partner>());

            // Act
            var result = await _sut.AddAsync(invitation, userId, CancellationToken.None);

            // Assert - Email should be sent to existing user with different template
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteExistingUserToBePartnerAdmin.ToString()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ExistingInvitation_AlreadyAccepted_ReturnsExisting()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).Build();

            var existingInvitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("already@test.com")
                .AsAccepted()
                .Build();

            var newInvitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("already@test.com")
                .Build();

            // Existing invitation
            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { existingInvitation });
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation> { existingInvitation });
            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _sut.AddAsync(newInvitation, userId, CancellationToken.None);

            // Assert - Should return existing invitation, not send any emails
            Assert.Equal((int)InvitationStatusEnum.Accepted, result.InvitationStatusId);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ExistingInvitation_NotAccepted_ResetsToNew()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).Build();

            var existingInvitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("resend@test.com")
                .AsDeclined() // Previously declined
                .Build();

            var newInvitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("resend@test.com")
                .Build();

            _invitationRepository.SetupGet(new List<PartnerAdminInvitation> { existingInvitation });
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation> { existingInvitation });
            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // User doesn't exist
            _userManager.Setup(m => m.GetUserByEmailAsync("resend@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            await _sut.AddAsync(newInvitation, userId, CancellationToken.None);

            // Assert - Should reset status to New and then send email
            _invitationRepository.Verify(r => r.UpdateAsync(It.Is<PartnerAdminInvitation>(
                i => i.InvitationStatusId == (int)InvitationStatusEnum.New)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ExistingUserAlreadyAdmin_SetsStatusToAccepted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).WithName("Test Partner").Build();
            var existingUser = new UserBuilder().WithEmail("admin@test.com").Build();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithPartnerId(partnerId)
                .WithEmail("admin@test.com")
                .Build();

            // No existing invitation
            _invitationRepository.SetupGet(new List<PartnerAdminInvitation>());
            _invitationRepository.SetupGetWithFilter(new List<PartnerAdminInvitation>());

            // User exists
            _userManager.Setup(m => m.GetUserByEmailAsync("admin@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // User is already an admin for this partner
            _partnerAdminManager.Setup(m => m.GetPartnersByUserIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Partner> { partner });

            // Act
            var result = await _sut.AddAsync(invitation, userId, CancellationToken.None);

            // Assert - No email should be sent since already accepted
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region ResendPartnerAdminInvitation Tests

        [Fact]
        public async Task ResendPartnerAdminInvitation_NewUser_SendsEmail()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).WithName("Test Partner").Build();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithId(invitationId)
                .WithPartnerId(partnerId)
                .WithEmail("resend@test.com")
                .AsSent()
                .Build();

            _invitationRepository.SetupGetAsync(invitation);
            _userManager.Setup(m => m.GetUserByEmailAsync("resend@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);
            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _sut.ResendPartnerAdminInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteNewUserToBePartnerAdmin.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("Administrator")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "resend@test.com")),
                It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal((int)InvitationStatusEnum.Sent, result.InvitationStatusId);
        }

        [Fact]
        public async Task ResendPartnerAdminInvitation_ExistingUser_SendsExistingUserEmail()
        {
            // Arrange
            var invitationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder().WithId(partnerId).WithName("Test Partner").Build();
            var existingUser = new UserBuilder()
                .WithEmail("existingresend@test.com")
                .WithUserName("ExistingUser")
                .Build();

            var invitation = new PartnerAdminInvitationBuilder()
                .WithId(invitationId)
                .WithPartnerId(partnerId)
                .WithEmail("existingresend@test.com")
                .AsSent()
                .Build();

            _invitationRepository.SetupGetAsync(invitation);
            _userManager.Setup(m => m.GetUserByEmailAsync("existingresend@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _sut.ResendPartnerAdminInvitationAsync(invitationId, userId, CancellationToken.None);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteExistingUserToBePartnerAdmin.ToString()), Times.Once);
            Assert.Equal((int)InvitationStatusEnum.Sent, result.InvitationStatusId);
        }

        #endregion
    }
}
