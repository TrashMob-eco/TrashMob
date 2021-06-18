namespace TrashMob.Shared.Tests
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using Xunit;

    public class NotificationEngineTests
    {
        [Fact]
        public async Task GenerateEventNotificatonsAsync_WithNoDataAvailable_Succeeds()
        {
            var engine = new NotificationEngine();

            await engine.GenerateEventNotificatonsAsync().ConfigureAwait(false);
        }
    }
}
