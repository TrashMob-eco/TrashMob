namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class EditLitterReportViewModelTests
{
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<IMapRestService> mockMapRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly EditLitterReportViewModel sut;

    public EditLitterReportViewModelTests()
    {
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockMapRestService = new Mock<IMapRestService>();
        mockNotificationService = new Mock<INotificationService>();

        sut = new EditLitterReportViewModel(
            mockLitterReportManager.Object,
            mockMapRestService.Object,
            mockNotificationService.Object);
    }

    [Fact]
    public async Task Init_LoadsLitterReport()
    {
        // Arrange
        var litterReportId = Guid.NewGuid();
        var testLitterReport = CreateTestLitterReportWithImages();
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(litterReportId);

        // Assert
        mockLitterReportManager.Verify(
            m => m.GetLitterReportAsync(litterReportId, ImageSizeEnum.Thumb, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_SetsNameAndDescription()
    {
        // Arrange
        var testLitterReport = CreateTestLitterReportWithImages();
        testLitterReport.Name = "Litter at Park";
        testLitterReport.Description = "Large pile of trash near playground";
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.Equal("Litter at Park", sut.Name);
        Assert.Equal("Large pile of trash near playground", sut.Description);
    }

    [Fact]
    public async Task Init_PopulatesLitterImageViewModels()
    {
        // Arrange
        var testLitterReport = CreateTestLitterReportWithImages(imageCount: 3);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.Equal(3, sut.LitterImageViewModels.Count);
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
    public void ValidateReport_ShortDescription_ReportIsNotValid()
    {
        // Arrange
        sut.Name = "Valid Name Here";
        sut.Description = "Short";
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
        for (var i = 0; i < EditLitterReportViewModel.MaxImages; i++)
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
    public void ValidateReport_LessThanMax_CanAddImages()
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

    private void SetupGetLitterReport(LitterReport litterReport)
    {
        mockLitterReportManager
            .Setup(m => m.GetLitterReportAsync(It.IsAny<Guid>(), It.IsAny<ImageSizeEnum>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(litterReport);
    }

    private static LitterReport CreateTestLitterReportWithImages(int imageCount = 2)
    {
        var testLitterReport = TestHelpers.CreateTestLitterReport();
        testLitterReport.LitterImages = new List<LitterImage>();

        for (var i = 0; i < imageCount; i++)
        {
            testLitterReport.LitterImages.Add(new LitterImage
            {
                Id = Guid.NewGuid(),
                LitterReportId = testLitterReport.Id,
                Latitude = 47.6062 + i * 0.001,
                Longitude = -122.3321 + i * 0.001,
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                PostalCode = "98101",
                StreetAddress = $"{100 + i} Main St",
                AzureBlobURL = $"https://example.blob.core.windows.net/image{i}.jpg",
                CreatedByUserId = Guid.NewGuid(),
                LastUpdatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            });
        }

        return testLitterReport;
    }
}
