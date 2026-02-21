namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMobMobile.Services;
using TrashMobMobile.ViewModels;
using Xunit;

public class WaiverViewModelTests
{
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IWaiverManager> mockWaiverManager;
    private readonly WaiverViewModel sut;

    public WaiverViewModelTests()
    {
        mockNotificationService = new Mock<INotificationService>();
        mockWaiverManager = new Mock<IWaiverManager>();

        sut = new WaiverViewModel(
            mockNotificationService.Object,
            mockWaiverManager.Object);
    }

    [Fact]
    public void Constructor_CreatesViewModel()
    {
        // Assert
        Assert.NotNull(sut);
    }

    [Fact]
    public void SignWaiverCommand_Exists()
    {
        // Assert
        Assert.NotNull(sut.SignWaiverCommand);
    }
}
