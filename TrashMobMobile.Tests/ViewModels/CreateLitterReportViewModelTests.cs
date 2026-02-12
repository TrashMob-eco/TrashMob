namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class CreateLitterReportViewModelTests
{
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<IMapRestService> mockMapRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly CreateLitterReportViewModel sut;

    public CreateLitterReportViewModelTests()
    {
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockMapRestService = new Mock<IMapRestService>();
        mockNotificationService = new Mock<INotificationService>();

        sut = new CreateLitterReportViewModel(
            mockLitterReportManager.Object,
            mockMapRestService.Object,
            mockNotificationService.Object);
    }

    [Fact]
    public void Constructor_SetsDefaultName()
    {
        // Assert
        Assert.Equal("New Litter Report", sut.Name);
    }

    [Fact]
    public void Constructor_SetsNewLitterReportStatus()
    {
        // Assert
        Assert.NotNull(sut.LitterReportViewModel);
        Assert.Equal(1, sut.LitterReportViewModel.LitterReportStatusId);
    }

    [Fact]
    public void ValidateReport_NoImages_ReportIsNotValid()
    {
        // Arrange
        sut.Name = "Valid Name Here";
        sut.Description = "Valid Description Here";

        // Act
        sut.ValidateReport();

        // Assert
        Assert.False(sut.ReportIsValid);
    }

    [Fact]
    public void ValidateReport_WithImages_ValidNameAndDescription_ReportIsValid()
    {
        // Arrange
        sut.Name = "Valid Name Here";
        sut.Description = "Valid Description Here";
        sut.LitterImageViewModels.Add(new LitterImageViewModel(mockNotificationService.Object));

        // Act
        sut.ValidateReport();

        // Assert
        Assert.True(sut.ReportIsValid);
    }

    [Fact]
    public void ValidateReport_ShortName_ReportIsNotValid()
    {
        // Arrange
        sut.Name = "Short";
        sut.Description = "Valid Description Here";
        sut.LitterImageViewModels.Add(new LitterImageViewModel(mockNotificationService.Object));

        // Act
        sut.ValidateReport();

        // Assert
        Assert.False(sut.ReportIsValid);
    }

    [Fact]
    public void ValidateReport_MaxImages_SetsHasMaxImages()
    {
        // Arrange
        for (var i = 0; i < CreateLitterReportViewModel.MaxImages; i++)
        {
            sut.LitterImageViewModels.Add(new LitterImageViewModel(mockNotificationService.Object));
        }

        // Act
        sut.ValidateReport();

        // Assert
        Assert.True(sut.HasMaxImages);
        Assert.False(sut.CanAddImages);
    }

    [Fact]
    public void ValidateReport_LessThanMax_SetsCanAddImages()
    {
        // Arrange
        sut.LitterImageViewModels.Add(new LitterImageViewModel(mockNotificationService.Object));

        // Act
        sut.ValidateReport();

        // Assert
        Assert.False(sut.HasMaxImages);
        Assert.True(sut.CanAddImages);
    }

    [Fact]
    public void Name_PropertyChanged_TriggersValidation()
    {
        // Arrange
        sut.LitterImageViewModels.Add(new LitterImageViewModel(mockNotificationService.Object));
        sut.Description = "Valid Description Here";

        // Act - setting Name triggers ValidateReport internally
        sut.Name = "Valid Name Here";

        // Assert
        Assert.True(sut.ReportIsValid);
    }

    [Fact]
    public void Description_PropertyChanged_TriggersValidation()
    {
        // Arrange
        sut.LitterImageViewModels.Add(new LitterImageViewModel(mockNotificationService.Object));
        sut.Name = "Valid Name Here";

        // Act - setting Description triggers ValidateReport internally
        sut.Description = "Valid Description Here";

        // Assert
        Assert.True(sut.ReportIsValid);
    }

    [Fact]
    public void ValidateReport_NoImages_HasNoImagesIsTrue()
    {
        // Act
        sut.ValidateReport();

        // Assert
        Assert.True(sut.HasNoImages);
    }
}
