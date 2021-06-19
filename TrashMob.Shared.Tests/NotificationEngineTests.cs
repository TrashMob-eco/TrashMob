namespace TrashMob.Shared.Tests
{
    using Moq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using Xunit;

    public class NotificationEngineTests
    {
        [Fact]
        public async Task GenerateEventNotificatonsAsync_WithNoDataAvailable_Succeeds()
        {
            var eventManager = new Mock<IEventManager>();
            var engine = new NotificationEngine(eventManager.Object);

            await engine.GenerateEventNotificatonsAsync().ConfigureAwait(false);
        }
    }
}
