namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class EditDependentViewModelTests
{
    private readonly Mock<IDependentRestService> mockDependentRestService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly User testUser;
    private readonly EditDependentViewModel sut;

    public EditDependentViewModelTests()
    {
        mockDependentRestService = new Mock<IDependentRestService>();
        mockUserManager = new Mock<IUserManager>();
        mockNotificationService = new Mock<INotificationService>();

        testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new EditDependentViewModel(
            mockDependentRestService.Object,
            mockUserManager.Object,
            mockNotificationService.Object);
    }

    // === Init Tests ===

    [Fact]
    public async Task Init_WithEmptyGuid_DoesNotLoadDependent()
    {
        // Act
        await sut.Init(Guid.Empty);

        // Assert
        Assert.False(sut.IsEditing);
        Assert.Equal("Add Dependent", sut.PageTitle);
        mockDependentRestService.Verify(
            m => m.GetDependentsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Init_WithValidId_LoadsDependentAndSetsEditMode()
    {
        // Arrange
        var dependent = TestHelpers.CreateTestDependent(
            parentUserId: testUser.Id,
            firstName: "Alice",
            lastName: "Smith",
            dateOfBirth: new DateOnly(2018, 5, 15),
            relationship: "Parent");
        dependent.MedicalNotes = "Peanut allergy";
        dependent.EmergencyContactPhone = "555-1234";

        mockDependentRestService
            .Setup(m => m.GetDependentsAsync(testUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([dependent]);

        // Act
        await sut.Init(dependent.Id);

        // Assert
        Assert.True(sut.IsEditing);
        Assert.Equal("Edit Dependent", sut.PageTitle);
        Assert.Equal("Alice", sut.FirstName);
        Assert.Equal("Smith", sut.LastName);
        Assert.Equal(new DateTime(2018, 5, 15), sut.DateOfBirth);
        Assert.Equal("Parent", sut.Relationship);
        Assert.Equal("Peanut allergy", sut.MedicalNotes);
        Assert.Equal("555-1234", sut.EmergencyContactPhone);
    }

    [Fact]
    public async Task Init_WithNonExistentId_DoesNotSetFields()
    {
        // Arrange
        mockDependentRestService
            .Setup(m => m.GetDependentsAsync(testUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await sut.Init(Guid.NewGuid());

        // Assert - ViewModel should be in edit mode but fields should remain defaults
        Assert.True(sut.IsEditing);
        Assert.Equal(string.Empty, sut.FirstName);
    }

    // === ValidateForm Tests ===

    [Fact]
    public void ValidateForm_WithValidData_SetsIsFormValidTrue()
    {
        // Arrange
        sut.FirstName = "Alice";
        sut.LastName = "Smith";
        sut.DateOfBirth = DateTime.Today.AddYears(-8);

        // Act
        sut.ValidateForm();

        // Assert
        Assert.True(sut.IsFormValid);
    }

    [Theory]
    [InlineData("", "Smith")]
    [InlineData("Alice", "")]
    [InlineData("  ", "Smith")]
    [InlineData("Alice", "   ")]
    public void ValidateForm_WithMissingNames_SetsIsFormValidFalse(string firstName, string lastName)
    {
        // Arrange
        sut.FirstName = firstName;
        sut.LastName = lastName;
        sut.DateOfBirth = DateTime.Today.AddYears(-8);

        // Act
        sut.ValidateForm();

        // Assert
        Assert.False(sut.IsFormValid);
    }

    [Fact]
    public void ValidateForm_WithFutureDateOfBirth_SetsIsFormValidFalse()
    {
        // Arrange
        sut.FirstName = "Alice";
        sut.LastName = "Smith";
        sut.DateOfBirth = DateTime.Today; // Not strictly before today

        // Act
        sut.ValidateForm();

        // Assert
        Assert.False(sut.IsFormValid);
    }

    // === Save Tests ===

    [Fact]
    public async Task SaveCommand_WhenAdding_CallsAddDependentAsync()
    {
        // Arrange
        sut.FirstName = "Alice";
        sut.LastName = "Smith";
        sut.DateOfBirth = DateTime.Today.AddYears(-8);
        sut.Relationship = "Parent";

        mockDependentRestService
            .Setup(m => m.AddDependentAsync(testUser.Id, It.IsAny<Dependent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dependent());

        // Act
        await sut.SaveCommand.ExecuteAsync(null);

        // Assert
        mockDependentRestService.Verify(
            m => m.AddDependentAsync(testUser.Id, It.Is<Dependent>(d =>
                d.FirstName == "Alice" &&
                d.LastName == "Smith" &&
                d.ParentUserId == testUser.Id &&
                d.Relationship == "Parent" &&
                d.IsActive), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveCommand_WhenEditing_CallsUpdateDependentAsync()
    {
        // Arrange
        var dependentId = Guid.NewGuid();
        var existing = TestHelpers.CreateTestDependent(id: dependentId, parentUserId: testUser.Id);

        mockDependentRestService
            .Setup(m => m.GetDependentsAsync(testUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([existing]);
        mockDependentRestService
            .Setup(m => m.UpdateDependentAsync(testUser.Id, It.IsAny<Dependent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dependent());

        await sut.Init(dependentId);

        sut.FirstName = "Updated";
        sut.LastName = "Name";
        sut.DateOfBirth = DateTime.Today.AddYears(-10);

        // Act
        await sut.SaveCommand.ExecuteAsync(null);

        // Assert
        mockDependentRestService.Verify(
            m => m.UpdateDependentAsync(testUser.Id, It.Is<Dependent>(d =>
                d.Id == dependentId &&
                d.FirstName == "Updated" &&
                d.LastName == "Name"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveCommand_WithInvalidForm_DoesNotCallService()
    {
        // Arrange — leave FirstName empty
        sut.LastName = "Smith";
        sut.DateOfBirth = DateTime.Today.AddYears(-8);

        // Act
        await sut.SaveCommand.ExecuteAsync(null);

        // Assert
        mockDependentRestService.Verify(
            m => m.AddDependentAsync(It.IsAny<Guid>(), It.IsAny<Dependent>(), It.IsAny<CancellationToken>()),
            Times.Never);
        mockDependentRestService.Verify(
            m => m.UpdateDependentAsync(It.IsAny<Guid>(), It.IsAny<Dependent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveCommand_TrimsWhitespace()
    {
        // Arrange
        sut.FirstName = "  Alice  ";
        sut.LastName = "  Smith  ";
        sut.DateOfBirth = DateTime.Today.AddYears(-8);
        sut.MedicalNotes = "  Notes  ";
        sut.EmergencyContactPhone = "  555-1234  ";

        mockDependentRestService
            .Setup(m => m.AddDependentAsync(testUser.Id, It.IsAny<Dependent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dependent());

        // Act
        await sut.SaveCommand.ExecuteAsync(null);

        // Assert
        mockDependentRestService.Verify(
            m => m.AddDependentAsync(testUser.Id, It.Is<Dependent>(d =>
                d.FirstName == "Alice" &&
                d.LastName == "Smith" &&
                d.MedicalNotes == "Notes" &&
                d.EmergencyContactPhone == "555-1234"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveCommand_EmptyOptionalFields_SetToNull()
    {
        // Arrange
        sut.FirstName = "Alice";
        sut.LastName = "Smith";
        sut.DateOfBirth = DateTime.Today.AddYears(-8);
        sut.MedicalNotes = "   ";
        sut.EmergencyContactPhone = "";

        mockDependentRestService
            .Setup(m => m.AddDependentAsync(testUser.Id, It.IsAny<Dependent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dependent());

        // Act
        await sut.SaveCommand.ExecuteAsync(null);

        // Assert
        mockDependentRestService.Verify(
            m => m.AddDependentAsync(testUser.Id, It.Is<Dependent>(d =>
                d.MedicalNotes == null &&
                d.EmergencyContactPhone == null), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // === Property Defaults ===

    [Fact]
    public void Relationships_ContainsExpectedOptions()
    {
        Assert.Contains("Parent", sut.Relationships);
        Assert.Contains("Legal Guardian", sut.Relationships);
        Assert.Contains("Grandparent", sut.Relationships);
        Assert.Contains("Authorized Supervisor", sut.Relationships);
        Assert.Contains("Other", sut.Relationships);
        Assert.Equal(5, sut.Relationships.Count);
    }

    [Fact]
    public void Today_ReturnsCurrentDate()
    {
        Assert.Equal(DateTime.Today, sut.Today);
    }

    [Fact]
    public void Default_RelationshipIsParent()
    {
        Assert.Equal("Parent", sut.Relationship);
    }

    [Fact]
    public void Default_IsNotEditing()
    {
        Assert.False(sut.IsEditing);
        Assert.Equal("Add Dependent", sut.PageTitle);
    }
}
