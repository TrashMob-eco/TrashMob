
namespace TrashMob.Shared.Tests
{
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Collections.Generic;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class NotifierTestsBase
    {
        private readonly Guid createdById;
        private readonly Guid userId1;
        private readonly Guid userId2;

        protected abstract NotificationTypeEnum NotificationType { get; }

        protected Mock<IEventManager> EventManager { get; }
        
        protected Mock<IEventAttendeeManager> EventAttendeeManager { get; }
        
        protected Mock<IKeyedManager<User>> UserManager { get; }
        
        protected Mock<IKeyedManager<UserNotification>> UserNotificationManager { get; }

        protected Mock<IKeyedManager<NonEventUserNotification>> NonEventUserNotificationManager { get; }

        protected Mock<IEmailSender> EmailSender { get; }

        protected Mock<IEmailManager> EmailManager { get; }

        protected Mock<IMapManager> MapRepository { get; }

        protected Mock<ILogger> Logger { get; }

        public NotifierTestsBase()
        {
            userId1 = Guid.NewGuid();
            userId2 = Guid.NewGuid();
            createdById = Guid.NewGuid();

            EventManager = new Mock<IEventManager>();
            EventAttendeeManager = new Mock<IEventAttendeeManager>();
            UserManager = new Mock<IKeyedManager<User>>();
            UserNotificationManager = new Mock<IKeyedManager<UserNotification>>();
            NonEventUserNotificationManager = new Mock<IKeyedManager<NonEventUserNotification>>();
            EmailSender = new Mock<IEmailSender>();
            EmailManager = new Mock<IEmailManager>();
            MapRepository = new Mock<IMapManager>();
            Logger = new Mock<ILogger>();

            // Setup a default return of distance between User and Event of 10 (in whatever units)
            MapRepository.Setup(mr => mr.GetDistanceBetweenTwoPointsAsync(It.IsAny<Tuple<double, double>>(), It.IsAny<Tuple<double, double>>(), It.IsAny<bool>())).ReturnsAsync(10);
            EmailManager.Setup(em => em.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test");
        }

        protected List<User> GetUserList1()
        {
            var user = new User()
            {
                City = "Seattle",
                Country = "United States",
                DateAgreedToPrivacyPolicy = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTermsOfService = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTrashMobWaiver = DateTimeOffset.UtcNow.AddDays(-2),
                Email = "testuser@trashmob.eco",
                GivenName = "Test",
                Id = userId1,
                MemberSince = DateTimeOffset.UtcNow.AddDays(-2),
                NameIdentifier = "123456789",
                PostalCode = "98040",
                PrivacyPolicyVersion = "1.0",
                Region = "Washington",
                SourceSystemUserName = "TestUser",
                SurName = "Bleg",
                TermsOfServiceVersion = "1.0",
                TrashMobWaiverVersion = "1.0",
                UserName = "BlegD",
                TravelLimitForLocalEvents = 25,
                Longitude = 1,
                Latitude = 1,
            };

            var users = new List<User>
            {
                user
            };

            return users;
        }

        protected List<User> GetUserList2()
        {
            var user1 = new User()
            {
                City = "Seattle",
                Country = "United States",
                DateAgreedToPrivacyPolicy = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTermsOfService = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTrashMobWaiver = DateTimeOffset.UtcNow.AddDays(-2),
                Email = "testuser@trashmob.eco",
                GivenName = "Test",
                Id = userId1,
                MemberSince = DateTimeOffset.UtcNow.AddDays(-2),
                NameIdentifier = "123456789",
                PostalCode = "98040",
                PrivacyPolicyVersion = "1.0",
                Region = "Washington",
                SourceSystemUserName = "TestUser",
                SurName = "Bleg",
                TermsOfServiceVersion = "1.0",
                TrashMobWaiverVersion = "1.0",
                UserName = "BlegD",
                TravelLimitForLocalEvents = 25,
                Longitude = 1,
                Latitude = 1,
            };

            var user2 = new User()
            {
                City = "Seattle",
                Country = "United States",
                DateAgreedToPrivacyPolicy = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTermsOfService = DateTimeOffset.UtcNow.AddDays(-2),
                DateAgreedToTrashMobWaiver = DateTimeOffset.UtcNow.AddDays(-2),
                Email = "testuser2@trashmob.eco",
                GivenName = "Test2",
                Id = userId2,
                MemberSince = DateTimeOffset.UtcNow.AddDays(-2),
                NameIdentifier = "12345678901232",
                PostalCode = "98040",
                PrivacyPolicyVersion = "1.0",
                Region = "Washington",
                SourceSystemUserName = "TestUser2",
                SurName = "Bleg2",
                TermsOfServiceVersion = "1.0",
                TrashMobWaiverVersion = "1.0",
                UserName = "BlegD2",
                TravelLimitForLocalEvents = 25,
                Longitude = 1,
                Latitude = 1,
            };

            var users = new List<User>
            {
                user1,
                user2,
            };

            return users;
        }

        protected List<Event> GetEventList1()
        {
            var relevantEvent = new Event()
            {
                City = "Seattle",
                Country = "United States",
                CreatedByUserId = createdById,
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-14),
                Description = "This is a test event",
                EventDate = DateTimeOffset.UtcNow.AddMinutes(10),
                EventStatusId = (int)EventStatusEnum.Active,
                EventTypeId = 3,
                Id = Guid.NewGuid(),
                LastUpdatedByUserId = createdById,
                LastUpdatedDate = DateTimeOffset.UtcNow.AddDays(-13),
                Latitude = 50,
                Longitude = 45,
                MaxNumberOfParticipants = 10,
                Name = "Test Event",
                PostalCode = "98040",
                Region = "Washington",
                StreetAddress = "1 King Street",
                IsEventPublic = true,
            };

            var events = new List<Event>
            {
                relevantEvent
            };

            return events;
        }

        protected List<Event> GetEventList2()
        {
            var relevantEvent1 = new Event()
            {
                City = "Seattle",
                Country = "United States",
                CreatedByUserId = createdById,
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-14),
                Description = "This is a test event",
                EventDate = DateTimeOffset.UtcNow.AddMinutes(10),
                EventStatusId = (int)EventStatusEnum.Active,
                EventTypeId = 3,
                Id = Guid.NewGuid(),
                LastUpdatedByUserId = createdById,
                LastUpdatedDate = DateTimeOffset.UtcNow.AddDays(-13),
                Latitude = 50,
                Longitude = 45,
                MaxNumberOfParticipants = 10,
                Name = "Test Event",
                PostalCode = "98040",
                Region = "Washington",
                StreetAddress = "1 King Street",
                IsEventPublic = true,
            };

            var relevantEvent2 = new Event()
            {
                City = "Seattle",
                Country = "United States",
                CreatedByUserId = createdById,
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-14),
                Description = "This is a test event",
                EventDate = DateTimeOffset.UtcNow.AddMinutes(20),
                EventStatusId = (int)EventStatusEnum.Active,
                EventTypeId = 3,
                Id = Guid.NewGuid(),
                LastUpdatedByUserId = createdById,
                LastUpdatedDate = DateTimeOffset.UtcNow.AddDays(-13),
                Latitude = 70,
                Longitude = 75,
                MaxNumberOfParticipants = 10,
                Name = "Test Event 2",
                PostalCode = "98040",
                Region = "Washington",
                StreetAddress = "1 Queen Street",
                IsEventPublic = true,
            };

            var events = new List<Event>
            {
                relevantEvent1,
                relevantEvent2,
            };

            return events;
        }
    }
}
