namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMobMobile.Services;
using TrashMobMobile.ViewModels;
using Xunit;

public class WaiverViewModelTests
{
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly WaiverViewModel sut;

    public WaiverViewModelTests()
    {
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        sut = new WaiverViewModel(
            mockNotificationService.Object,
            mockUserManager.Object);
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
