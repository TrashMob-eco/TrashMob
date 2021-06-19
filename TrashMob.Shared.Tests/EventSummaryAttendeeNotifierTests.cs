namespace TrashMob.Shared.Tests
{
    using Moq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Persistence;
    using Xunit;

    public class EventSummaryAttendeeNotifierTests
    {
        private readonly Mock<IEventRepository> eventRepository;
        private readonly Mock<IEventAttendeeRepository> eventAttendeeRepository;
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IEmailSender> emailSender;

        public EventSummaryAttendeeNotifierTests()
        {
            eventRepository = new Mock<IEventRepository>();
            eventAttendeeRepository = new Mock<IEventAttendeeRepository>();
            userRepository = new Mock<IUserRepository>();
            emailSender = new Mock<IEmailSender>();
        }

        [Fact]
        public async Task GenerateNotificationsAsync_WithNoDataAvailable_Succeeds()
        {
            var engine = new EventSummaryAttendeeNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, emailSender.Object);

            await engine.GenerateNotificationsAsync().ConfigureAwait(false);
        }

        [Fact]
        public void GetEmailTemplate_Succeeds()
        {
            var engine = new EventSummaryAttendeeNotifier(eventRepository.Object, userRepository.Object, eventAttendeeRepository.Object, emailSender.Object);

            var template = engine.GetEmailTemplate();

            Assert.False(string.IsNullOrWhiteSpace(template));
        }
    }
}
