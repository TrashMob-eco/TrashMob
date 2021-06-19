namespace TrashMob.Shared.Tests
{
    using Moq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Persistence;
    using Xunit;

    public class UpcomingEventHostingThisWeekNotifierTests
    {
        private readonly Mock<IEventRepository> eventRepository;
        private readonly Mock<IEventAttendeeRepository> eventAttendeeRepository;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IEmailSender> emailSender;

        public UpcomingEventHostingThisWeekNotifierTests()
        {
            eventRepository = new Mock<IEventRepository>();
            eventAttendeeRepository = new Mock<IEventAttendeeRepository>();
            userRepository = new Mock<IUserRepository>();
            emailSender = new Mock<IEmailSender>();
        }

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            var engine = new UpcomingEventHostingThisWeekNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, emailSender.Object);

            await engine.GenerateNotificationsAsync().ConfigureAwait(false);
        }
    }
}
