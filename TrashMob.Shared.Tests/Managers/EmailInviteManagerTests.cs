namespace TrashMob.Shared.Tests.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="EmailInviteManager"/>.
    /// </summary>
    public class EmailInviteManagerTests
    {
        private readonly Mock<IKeyedRepository<EmailInviteBatch>> _batchRepository;
        private readonly Mock<IKeyedRepository<EmailInvite>> _inviteRepository;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly Mock<IKeyedManager<Partner>> _partnerManager;
        private readonly Mock<ITeamManager> _teamManager;
        private readonly EmailInviteManager _sut;

        public EmailInviteManagerTests()
        {
            _batchRepository = new Mock<IKeyedRepository<EmailInviteBatch>>();
            _inviteRepository = new Mock<IKeyedRepository<EmailInvite>>();
            _emailManager = new Mock<IEmailManager>();
            _partnerManager = new Mock<IKeyedManager<Partner>>();
            _teamManager = new Mock<ITeamManager>();

            // Default setup
            _batchRepository.SetupAddAsync();
            _batchRepository.SetupUpdateAsync();
            _inviteRepository.SetupAddAsync();
            _inviteRepository.SetupUpdateAsync();

            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new EmailInviteManager(
                _batchRepository.Object,
                _inviteRepository.Object,
                _emailManager.Object,
                _partnerManager.Object,
                _teamManager.Object);
        }

        #region CreateBatchAsync Tests

        [Fact]
        public async Task CreateBatchAsync_CreatesValidBatch()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var emails = new[] { "user1@test.com", "user2@test.com", "user3@test.com" };

            EmailInviteBatch capturedBatch = null;
            _batchRepository.Setup(r => r.AddAsync(It.IsAny<EmailInviteBatch>()))
                .Callback<EmailInviteBatch>(b => capturedBatch = b)
                .ReturnsAsync((EmailInviteBatch b) => b);

            // Act
            var result = await _sut.CreateBatchAsync(emails, senderUserId, "Admin");

            // Assert
            Assert.NotNull(capturedBatch);
            Assert.Equal(senderUserId, capturedBatch.SenderUserId);
            Assert.Equal("Admin", capturedBatch.BatchType);
            Assert.Equal(3, capturedBatch.TotalCount);
            Assert.Equal(0, capturedBatch.SentCount);
            Assert.Equal("Pending", capturedBatch.Status);
        }

        [Fact]
        public async Task CreateBatchAsync_NormalizesEmails()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var emails = new[] { "  USER@Test.Com  ", "user@test.com", "ANOTHER@TEST.COM" };

            var createdInvites = new List<EmailInvite>();
            _inviteRepository.Setup(r => r.AddAsync(It.IsAny<EmailInvite>()))
                .Callback<EmailInvite>(i => createdInvites.Add(i))
                .ReturnsAsync((EmailInvite i) => i);

            // Act
            await _sut.CreateBatchAsync(emails, senderUserId, "Admin");

            // Assert - Should deduplicate after normalization
            Assert.Equal(2, createdInvites.Count);
            Assert.Contains(createdInvites, i => i.Email == "user@test.com");
            Assert.Contains(createdInvites, i => i.Email == "another@test.com");
        }

        [Fact]
        public async Task CreateBatchAsync_FiltersInvalidEmails()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var emails = new[] { "valid@test.com", "invalid-no-at", "", "  ", null, "another@test.com" };

            var createdInvites = new List<EmailInvite>();
            _inviteRepository.Setup(r => r.AddAsync(It.IsAny<EmailInvite>()))
                .Callback<EmailInvite>(i => createdInvites.Add(i))
                .ReturnsAsync((EmailInvite i) => i);

            // Act
            await _sut.CreateBatchAsync(emails, senderUserId, "Admin");

            // Assert
            Assert.Equal(2, createdInvites.Count);
            Assert.All(createdInvites, i => Assert.Contains("@", i.Email));
        }

        [Fact]
        public async Task CreateBatchAsync_ThrowsWhenNoValidEmails()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var emails = new[] { "invalid", "", null };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateBatchAsync(emails, senderUserId, "Admin"));
            Assert.Contains("No valid email", ex.Message);
        }

        [Fact]
        public async Task CreateBatchAsync_SetsCommunityId()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            var emails = new[] { "user@test.com" };

            EmailInviteBatch capturedBatch = null;
            _batchRepository.Setup(r => r.AddAsync(It.IsAny<EmailInviteBatch>()))
                .Callback<EmailInviteBatch>(b => capturedBatch = b)
                .ReturnsAsync((EmailInviteBatch b) => b);

            // Act
            await _sut.CreateBatchAsync(emails, senderUserId, "Community", communityId);

            // Assert
            Assert.Equal(communityId, capturedBatch.CommunityId);
            Assert.Null(capturedBatch.TeamId);
        }

        [Fact]
        public async Task CreateBatchAsync_SetsTeamId()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var emails = new[] { "user@test.com" };

            EmailInviteBatch capturedBatch = null;
            _batchRepository.Setup(r => r.AddAsync(It.IsAny<EmailInviteBatch>()))
                .Callback<EmailInviteBatch>(b => capturedBatch = b)
                .ReturnsAsync((EmailInviteBatch b) => b);

            // Act
            await _sut.CreateBatchAsync(emails, senderUserId, "Team", teamId: teamId);

            // Assert
            Assert.Equal(teamId, capturedBatch.TeamId);
            Assert.Null(capturedBatch.CommunityId);
        }

        [Fact]
        public async Task CreateBatchAsync_CreatesIndividualInvites()
        {
            // Arrange
            var senderUserId = Guid.NewGuid();
            var emails = new[] { "user1@test.com", "user2@test.com" };

            var createdInvites = new List<EmailInvite>();
            _inviteRepository.Setup(r => r.AddAsync(It.IsAny<EmailInvite>()))
                .Callback<EmailInvite>(i => createdInvites.Add(i))
                .ReturnsAsync((EmailInvite i) => i);

            // Act
            var batch = await _sut.CreateBatchAsync(emails, senderUserId, "Admin");

            // Assert
            Assert.Equal(2, createdInvites.Count);
            Assert.All(createdInvites, i => Assert.Equal(batch.Id, i.BatchId));
            Assert.All(createdInvites, i => Assert.Equal("Pending", i.Status));
        }

        #endregion

        #region ProcessBatchAsync Tests

        [Fact]
        public async Task ProcessBatchAsync_ThrowsWhenBatchNotFound()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _batchRepository.SetupGet(new List<EmailInviteBatch>());
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.ProcessBatchAsync(batchId));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task ProcessBatchAsync_UpdatesStatusToProcessing()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var invite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .WithInvites(new List<EmailInvite> { invite })
                .AsPending()
                .Build();

            var batches = new List<EmailInviteBatch> { batch };
            _batchRepository.SetupGet(batches);
            _batchRepository.SetupGetWithFilter(batches);

            EmailInviteBatch firstUpdate = null;
            _batchRepository.Setup(r => r.UpdateAsync(It.IsAny<EmailInviteBatch>()))
                .Callback<EmailInviteBatch>(b =>
                {
                    if (firstUpdate == null) firstUpdate = new EmailInviteBatch { Status = b.Status };
                })
                .ReturnsAsync((EmailInviteBatch b) => b);

            // Act
            await _sut.ProcessBatchAsync(batchId);

            // Assert
            Assert.Equal("Processing", firstUpdate?.Status);
        }

        [Fact]
        public async Task ProcessBatchAsync_SendsEmailsForPendingInvites()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var invite1 = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test1@example.com")
                .AsPending()
                .Build();
            var invite2 = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test2@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .WithInvites(new List<EmailInvite> { invite1, invite2 })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });

            // Act
            await _sut.ProcessBatchAsync(batchId);

            // Assert
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessBatchAsync_UpdatesStatusToComplete()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var invite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .WithInvites(new List<EmailInvite> { invite })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });

            // Act
            var result = await _sut.ProcessBatchAsync(batchId);

            // Assert
            Assert.Equal("Complete", result.Status);
            Assert.NotNull(result.CompletedDate);
        }

        [Fact]
        public async Task ProcessBatchAsync_SkipsAlreadySentInvites()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var pendingInvite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("pending@example.com")
                .AsPending()
                .Build();
            var sentInvite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("sent@example.com")
                .AsSent()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .WithInvites(new List<EmailInvite> { pendingInvite, sentInvite })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });

            // Act
            await _sut.ProcessBatchAsync(batchId);

            // Assert - Only 1 email should be sent (for pending invite)
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.Is<List<EmailAddress>>(list => list.Exists(a => a.Email == "pending@example.com")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessBatchAsync_HandlesEmailFailure()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var invite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("fail@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .WithInvites(new List<EmailInvite> { invite })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });

            // Make email sending fail
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Email send failed"));

            // Act
            var result = await _sut.ProcessBatchAsync(batchId);

            // Assert
            Assert.Equal(1, result.FailedCount);
            Assert.Equal("Failed", invite.Status);
            Assert.Contains("Email send failed", invite.ErrorMessage);
        }

        [Fact]
        public async Task ProcessBatchAsync_CommunityBatch_UsesCommunityTemplate()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var community = new PartnerBuilder().WithId(communityId).WithName("Test Community").Build();
            var invite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .AsCommunityBatch(communityId)
                .WithInvites(new List<EmailInvite> { invite })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });
            _partnerManager.Setup(m => m.GetAsync(communityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(community);

            // Act
            await _sut.ProcessBatchAsync(batchId);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteToJoinCommunity.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("Test Community")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessBatchAsync_TeamBatch_UsesTeamTemplate()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var team = new TeamBuilder().WithId(teamId).WithName("Test Team").Build();
            var invite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .AsTeamBatch(teamId)
                .WithInvites(new List<EmailInvite> { invite })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });
            _teamManager.Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            // Act
            await _sut.ProcessBatchAsync(batchId);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteToJoinTeam.ToString()), Times.Once);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("Test Team")),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessBatchAsync_GenericBatch_UsesTrashMobTemplate()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var sender = new UserBuilder().WithUserName("TestSender").Build();
            var invite = new EmailInviteBuilder()
                .WithBatchId(batchId)
                .WithEmail("test@example.com")
                .AsPending()
                .Build();

            var batch = new EmailInviteBatchBuilder()
                .WithId(batchId)
                .WithSenderUser(sender)
                .AsAdminBatch() // No community or team
                .WithInvites(new List<EmailInvite> { invite })
                .AsPending()
                .Build();

            _batchRepository.SetupGet(new List<EmailInviteBatch> { batch });
            _batchRepository.SetupGetWithFilter(new List<EmailInviteBatch> { batch });

            // Act
            await _sut.ProcessBatchAsync(batchId);

            // Assert
            _emailManager.Verify(e => e.GetHtmlEmailCopy(NotificationTypeEnum.InviteToJoinTrashMob.ToString()), Times.Once);
        }

        #endregion

        #region GetUserMonthlyInviteCountAsync Tests

        [Fact]
        public async Task GetUserMonthlyInviteCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentMonth = DateTimeOffset.UtcNow;

            var batch = new EmailInviteBatchBuilder()
                .WithSender(userId)
                .AsUserBatch()
                .Build();
            batch.CreatedDate = currentMonth;

            var invite1 = new EmailInviteBuilder().WithBatch(batch).Build();
            var invite2 = new EmailInviteBuilder().WithBatch(batch).Build();

            var invites = new List<EmailInvite> { invite1, invite2 };
            _inviteRepository.SetupGet(invites);
            _inviteRepository.SetupGetWithFilter(invites);

            // Act
            var result = await _sut.GetUserMonthlyInviteCountAsync(userId);

            // Assert
            Assert.Equal(2, result);
        }

        #endregion

        #region GetUserBatchesAsync Tests

        [Fact]
        public async Task GetUserBatchesAsync_ReturnsOnlyUserBatches()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sender = new UserBuilder().WithId(userId).Build();

            var userBatch = new EmailInviteBatchBuilder()
                .WithSender(userId)
                .AsUserBatch()
                .Build();

            var adminBatch = new EmailInviteBatchBuilder()
                .WithSender(userId)
                .AsAdminBatch()
                .Build();

            var batches = new List<EmailInviteBatch> { userBatch, adminBatch };
            _batchRepository.SetupGet(batches);
            _batchRepository.SetupGetWithFilter(batches);

            // Act
            var result = await _sut.GetUserBatchesAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("User", resultList[0].BatchType);
        }

        #endregion

        #region GetCommunityBatchesAsync Tests

        [Fact]
        public async Task GetCommunityBatchesAsync_ReturnsOnlyCommunityBatches()
        {
            // Arrange
            var communityId = Guid.NewGuid();
            var otherCommunityId = Guid.NewGuid();

            var batch1 = new EmailInviteBatchBuilder()
                .AsCommunityBatch(communityId)
                .Build();

            var batch2 = new EmailInviteBatchBuilder()
                .AsCommunityBatch(otherCommunityId)
                .Build();

            var batches = new List<EmailInviteBatch> { batch1, batch2 };
            _batchRepository.SetupGet(batches);
            _batchRepository.SetupGetWithFilter(batches);

            // Act
            var result = await _sut.GetCommunityBatchesAsync(communityId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(communityId, resultList[0].CommunityId);
        }

        #endregion

        #region GetTeamBatchesAsync Tests

        [Fact]
        public async Task GetTeamBatchesAsync_ReturnsOnlyTeamBatches()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var otherTeamId = Guid.NewGuid();

            var batch1 = new EmailInviteBatchBuilder()
                .AsTeamBatch(teamId)
                .Build();

            var batch2 = new EmailInviteBatchBuilder()
                .AsTeamBatch(otherTeamId)
                .Build();

            var batches = new List<EmailInviteBatch> { batch1, batch2 };
            _batchRepository.SetupGet(batches);
            _batchRepository.SetupGetWithFilter(batches);

            // Act
            var result = await _sut.GetTeamBatchesAsync(teamId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(teamId, resultList[0].TeamId);
        }

        #endregion
    }
}
