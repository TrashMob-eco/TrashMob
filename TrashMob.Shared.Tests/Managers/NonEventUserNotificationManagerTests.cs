namespace TrashMob.Shared.Tests.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class NonEventUserNotificationManagerTests
    {
        private readonly Mock<IKeyedRepository<NonEventUserNotification>> _notificationRepository;
        private readonly NonEventUserNotificationManager _sut;

        public NonEventUserNotificationManagerTests()
        {
            _notificationRepository = new Mock<IKeyedRepository<NonEventUserNotification>>();
            _sut = new NonEventUserNotificationManager(_notificationRepository.Object);
        }

        #region GetByUserIdAsync

        [Fact]
        public async Task GetByUserIdAsync_ReturnsNotificationsForUserAndType()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var notificationType = 1;
            var notification1 = CreateNotification(userId, notificationType);
            var notification2 = CreateNotification(userId, notificationType);
            var differentTypeNotification = CreateNotification(userId, 2);
            var differentUserNotification = CreateNotification(Guid.NewGuid(), notificationType);

            var notifications = new List<NonEventUserNotification>
            {
                notification1, notification2, differentTypeNotification, differentUserNotification
            };
            _notificationRepository.SetupGetWithFilter(notifications);

            // Act
            var result = await _sut.GetByUserIdAsync(userId, notificationType);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, n =>
            {
                Assert.Equal(userId, n.UserId);
                Assert.Equal(notificationType, n.UserNotificationTypeId);
            });
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsEmptyWhenNoNotifications()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var notificationType = 1;

            _notificationRepository.SetupGetWithFilter(new List<NonEventUserNotification>());

            // Act
            var result = await _sut.GetByUserIdAsync(userId, notificationType);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_FiltersCorrectlyByNotificationType()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var type1Notification = CreateNotification(userId, 1);
            var type2Notification = CreateNotification(userId, 2);

            var notifications = new List<NonEventUserNotification> { type1Notification, type2Notification };
            _notificationRepository.SetupGetWithFilter(notifications);

            // Act
            var result = await _sut.GetByUserIdAsync(userId, 2);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(2, resultList[0].UserNotificationTypeId);
        }

        #endregion

        private static NonEventUserNotification CreateNotification(Guid userId, int notificationType)
        {
            var creatorId = Guid.NewGuid();
            return new NonEventUserNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserNotificationTypeId = notificationType,
                SentDate = DateTimeOffset.UtcNow,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
