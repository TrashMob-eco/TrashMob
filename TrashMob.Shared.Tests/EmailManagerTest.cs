namespace TrashMob.Shared.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Collections.Generic;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence.Interfaces;
    using Xunit;

    public class EmailManagerTest  
    {
        private readonly IEmailManager emailManager;

        public EmailManagerTest()
        {
            var configuration = new Mock<IConfiguration>().Object;
            var emailSender = new Mock<IEmailSender>().Object;
            var logger = new Mock<ILogger<EmailManager>>().Object;

            emailManager = new EmailManager(configuration, emailSender, logger);
        }

        [Theory]
        [MemberData(nameof(enumValues))]
        public void GetHtmlEmailCopy(string inputNotificationType)
        {
            var emailCopy = emailManager.GetHtmlEmailCopy(inputNotificationType);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(emailCopy));
        }

        public static IEnumerable<object[]> enumValues()
        {
            foreach (var notificationType in Enum.GetValues(typeof(NotificationTypeEnum)))
            {
                yield return new object[] { notificationType.ToString() };
            }
        }
    }
}

