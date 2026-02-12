namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMobMobile.Services;
using TrashMobMobile.ViewModels;
using Xunit;

public class ContactUsViewModelTests
{
    private readonly Mock<IContactRequestManager> mockContactRequestManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly ContactUsViewModel sut;

    public ContactUsViewModelTests()
    {
        mockContactRequestManager = new Mock<IContactRequestManager>();
        mockNotificationService = new Mock<INotificationService>();

        sut = new ContactUsViewModel(
            mockContactRequestManager.Object,
            mockNotificationService.Object);
    }

    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Assert
        Assert.Equal(string.Empty, sut.Name);
        Assert.Equal(string.Empty, sut.Email);
        Assert.Equal(string.Empty, sut.Message);
        Assert.Equal(string.Empty, sut.Confirmation);
    }

    [Fact]
    public void Name_CanBeSet()
    {
        // Act
        sut.Name = "John Doe";

        // Assert
        Assert.Equal("John Doe", sut.Name);
    }

    [Fact]
    public void Email_CanBeSet()
    {
        // Act
        sut.Email = "john@example.com";

        // Assert
        Assert.Equal("john@example.com", sut.Email);
    }

    [Fact]
    public void Message_CanBeSet()
    {
        // Act
        sut.Message = "I would like to volunteer.";

        // Assert
        Assert.Equal("I would like to volunteer.", sut.Message);
    }
}
